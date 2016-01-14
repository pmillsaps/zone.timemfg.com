using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Time.Data.EntityModels.ITInventory;
using Time.Data.Models.MessageQueue;
using Time.IT.ViewModel;

namespace Time.IT.Controllers
{
    [Themed]
    [Authorize]
    public class ComputersController : Controller
    {
        private ITInventoryEntities db = new ITInventoryEntities();

        // GET: Computers
        public ActionResult Index(string search = "")
        {
            var computers = db.Computers.OrderBy(x => x.Name).Include(c => c.Ref_Memory).Include(c => c.Ref_Model).Include(c => c.Ref_Status).Include(c => c.Ref_VideoCard).Include(c => c.Ref_DeviceType).Include(c => c.Ref_Processor).Include(c => c.Ref_Sound).Include(c => c.Ref_OS);
            if (!String.IsNullOrEmpty(search))
                computers = computers.Where(x => x.User.Name.Contains(search) || x.Name.Contains(search)
                    || x.Ref_Model.Model.Contains(search) || x.Ref_OS.OS.Contains(search) || x.Ref_DeviceType.DeviceType.Contains(search) || x.SerialNumber.Contains(search));

            ViewBag.SearchIncludes = "Search Includes: User Name, Computer Name, Model, OS, Device Type, Serial Number";
            return View(computers.ToList());
        }

        // GET: Computers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Computer computer = db.Computers.Find(id);
            if (computer == null)
            {
                return HttpNotFound();
            }

            ViewBag.ComputerId = id;
            return View(computer);
        }

        public ActionResult Ping(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NIC nic = db.Ref_NIC.Find(id);
            if (nic == null)
            {
                return HttpNotFound();
            }

            Ping ping = new Ping();
            string pingAddress = nic.IP;
            if (string.IsNullOrEmpty(pingAddress) || pingAddress.ToUpper() == "DHCP") pingAddress = nic.Computer.Name;
            PingReply pingreply = ping.Send(pingAddress);

            if (pingreply.Status == IPStatus.Success)
                ViewBag.StatusMessage = "Alive";
            else
                ViewBag.ErrorMessage = "No Reply";

            Computer computer = db.Computers.Find(nic.Computer.Id);

            return View("Details", computer);
        }

        public ActionResult Wake(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ref_NIC nic = db.Ref_NIC.Find(id);
            if (nic == null)
            {
                return HttpNotFound();
            }

            string pingAddress = nic.MAC;
            bool success = WakeLan(pingAddress);
            var command = new WOLMessage
            {
                IP = nic.IP,
                MAC = nic.MAC
            };
            success = MSMQ.SendQueueMessage(command, MessageType.WOL);

            if (success)
                ViewBag.StatusMessage = "WOL Packet Sent";
            else
                ViewBag.ErrorMessage = "Error Sending WOL Packet";

            Computer computer = db.Computers.Find(nic.Computer.Id);

            return View("Details", computer);
        }

        public bool WakeLan(string macAddress)
        {
            if (macAddress.Length == 12 && !(macAddress.Contains(":") || macAddress.Contains("-")))
            {
                string tmp = String.Empty;
                for (int i = 0; i < macAddress.Length; i += 2)
                {
                    tmp += macAddress.Substring(i, 2) + ":";
                }
                tmp = tmp.Trim(':');
                macAddress = tmp;
            }

            string[] byteStrings = macAddress.Split(':');
            if (byteStrings.Count() == 0) byteStrings = macAddress.Split('-');

            byte[] bytes = new byte[6];

            for (int i = 0; i < 6; i++)
                bytes[i] = (byte)Int32.Parse(byteStrings[i], System.Globalization.NumberStyles.HexNumber);

            WakeLan2(bytes);
            return true;
        }

        public void WakeLan2(byte[] macAddress)
        {
            if (macAddress == null)
            {
                throw new ArgumentNullException("macAddress", "MAC Address must be provided");
            }

            if (macAddress.Length != 6)
            {
                throw new ArgumentOutOfRangeException("macAddress", "MAC Address must have 6 bytes");
            }

            // A Wake on LAN magic packet contains a 6 byte header and
            // the MAC address of the target MAC address (6 bytes) 16 times
            byte[] wolPacket = new byte[17 * 6];

            MemoryStream ms = new MemoryStream(wolPacket, true);

            // Write the 6 byte 0xFF header
            for (int i = 0; i < 6; i++)
            {
                ms.WriteByte(0xFF);
            }

            // Write the MAC Address 16 times
            for (int i = 0; i < 16; i++)
            {
                ms.Write(macAddress, 0, macAddress.Length);
            }

            // Broadcast the magic packet
            UdpClient udp = new UdpClient();
            udp.Connect(IPAddress.Broadcast, 9);
            udp.Send(wolPacket, wolPacket.Length);
        }

        // GET: Computers/Create
        public ActionResult Create()
        {
            GenerateDropDowns(new Computer());
            return View();
        }

        // POST: Computers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Computer computer)
        {
            computer.LastDateEdited = DateTime.Now;
            computer.LastEditedBy = System.Web.HttpContext.Current.User.Identity.Name;

            if (ModelState.IsValid)
            {
                db.Computers.Add(computer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            GenerateDropDowns(computer);
            return View(computer);
        }

        // GET: Computers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Computer computer = db.Computers.Find(id);
            if (computer == null)
            {
                return HttpNotFound();
            }
            GenerateDropDowns(computer);
            return View(computer);
        }

        // POST: Computers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Computer computer)
        {
            if (ModelState.IsValid)
            {
                computer.LastDateEdited = DateTime.Now;
                computer.LastEditedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.Entry(computer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            GenerateDropDowns(computer);
            return View(computer);
        }

        // GET: Computers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Computer computer = db.Computers.Find(id);
            if (computer == null)
            {
                return HttpNotFound();
            }
            return View(computer);
        }

        // POST: Computers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Computer computer = db.Computers.Find(id);
            db.Computers.Remove(computer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult _NIC(int id)
        {
            var computer = db.Computers.FirstOrDefault(x => x.Id == id);
            IEnumerable<Ref_NIC> nics;
            if (computer != null)
            {
                nics = computer.Ref_NIC;
                return PartialView(nics);
            }
            return null;
        }

        // GET: Ref_NIC/Create
        public ActionResult AddNIC(int id)
        {
            Ref_NIC nic = new Ref_NIC
            {
                ComputerId = id
            };
            SetNICDropDowns(nic);

            return View(nic);
        }

        private void SetNICDropDowns(Ref_NIC nic)
        {
            //var usedCables = db.Ref_NIC.Select(x => x.CableId);
            //var usedPorts = db.Ref_SwitchPort.Select(x => x.Id);
            var usedCables = db.Ref_NIC.Where(x => x.CableId != null).Select(x => x.CableId).ToList();
            var usedPorts = db.Ref_NIC.Where(x => x.Ref_SwitchPort != null).Select(x => x.SwitchPortId).ToList();
            ViewBag.CableId = new SelectList(db.Ref_CableNo.Where(x => !usedCables.Contains(x.Id)).OrderBy(x => x.Name), "Id", "Name", nic.CableId);
            ViewBag.SpeedId = new SelectList(db.Ref_NICSpeed.OrderBy(x => x.NIC_Speed), "Id", "NIC_Speed", nic.SpeedId);
            ViewBag.SwitchPortId = new SelectList(db.Ref_SwitchPort.Where(x => !usedPorts.Contains(x.Id)).OrderBy(x => x.SwitchPort), "Id", "SwitchPort", nic.SwitchPortId);
            ViewBag.ComputerId = new SelectList(db.Computers.OrderBy(x => x.Name), "Id", "Name", nic.ComputerId);
        }

        // POST: Ref_NIC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNIC([Bind(Exclude = "Id")] Ref_NIC ref_NIC)
        {
            ref_NIC.MAC = ref_NIC.MAC.Replace(":", "").Replace("-", "").Replace(" ", "");
            ValidateNIC(ref_NIC);
            if (ModelState.IsValid)
            {
                var returnId = ref_NIC.ComputerId;
                db.Ref_NIC.Add(ref_NIC);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = returnId });
            }
            SetNICDropDowns(ref_NIC);

            return View(ref_NIC);
        }

        private void ValidateNIC(Ref_NIC ref_NIC)
        {
            if (ref_NIC.MAC.Length != 12) ModelState.AddModelError("MAC", "MAC does not conform to the standard length");
            if (Regex.Match("^[a-fA-F0-9]{12}$", ref_NIC.MAC).Success) ModelState.AddModelError("MAC", "MAC does not pass the regex validation");
            if (db.Ref_NIC.Where(x => x.MAC == ref_NIC.MAC).Count() > 0) ModelState.AddModelError("MAC", "MAC is a duplicate of an existing NIC entry");
            if (!String.IsNullOrEmpty(ref_NIC.IP) && ref_NIC.IP.ToUpper() != "DHCP")
            {
                if (!Regex.IsMatch(ref_NIC.IP, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$")) ModelState.AddModelError("IP", "IP address is not formatted correctly.");
                if (db.Ref_NIC.Where(x => x.IP == ref_NIC.IP).Count() > 0) ModelState.AddModelError("IP", "IP address is a duplicate of an existing NIC entry");
                foreach (var item in ref_NIC.IP.Split('.'))
                {
                    int number;
                    if (Int32.TryParse(item, out number))
                    {
                        if (number < 1 || number > 255) ModelState.AddModelError("IP", "IP address segment is out of range");
                    }
                    else
                    {
                        ModelState.AddModelError("IP", "IP address segment is not a number");
                    }
                }
            }
        }

        public ActionResult LinkLicense(int id)
        {
            var usedLicenses = db.Computers.Find(id).Licenses.Select(x => x.Id);

            ViewBag.LicenseId = new SelectList(db.Licenses.Where(x => !usedLicenses.Contains(x.Id) && x.QuantityAssigned < x.Quantity).OrderBy(x => x.Name).ThenBy(x => x.LicenseKey), "Id", "FullName");
            LinkLicenseViewModel vm = new LinkLicenseViewModel
            {
                ComputerId = id,
                //Monitors = new SelectList(db.Monitors.Where(x => x.UserId == null).OrderBy(x => x.SerialNo), "Id", "SerialNo")
            };
            return View(vm);
        }

        // POST: Monitors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLicense(LinkLicenseViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var comp = db.Computers.Find(vm.ComputerId);
                var license = db.Licenses.Find(vm.LicenseId);
                license.QuantityAssigned++;
                comp.Licenses.Add(license);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = vm.ComputerId });
            }

            return View(vm);
        }

        public ActionResult UnLinkLicense(int id, int ComputerId)
        {
            var license = db.Licenses.Find(id);
            var comp = db.Computers.Find(ComputerId);
            comp.Licenses.Remove(license);
            license.QuantityAssigned--;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = comp.Id });
        }

        // GET: ScheduledTasks/Details/5
        public ActionResult DetailsScheduledTask(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledTask scheduledTask = db.ScheduledTasks.Find(id);
            if (scheduledTask == null)
            {
                return HttpNotFound();
            }
            return View(scheduledTask);
        }

        // GET: ScheduledTasks/Create
        public ActionResult AddScheduledTask(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Computer computer = db.Computers.Find(id);
            if (computer == null)
            {
                return HttpNotFound();
            }

            ViewBag.ComputerName = computer.Name;
            ScheduledTask task = new ScheduledTask { ComputerId = id };
            return View(task);
        }

        // POST: ScheduledTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddScheduledTask([Bind(Exclude = "Id")] ScheduledTask scheduledTask)
        {
            if (ModelState.IsValid)
            {
                scheduledTask.LastUpdatedDate = DateTime.Now;
                scheduledTask.LastUpdatedBy = System.Web.HttpContext.Current.User.Identity.Name;

                db.ScheduledTasks.Add(scheduledTask);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = scheduledTask.ComputerId });
            }

            ViewBag.ComputerId = new SelectList(db.Computers, "Id", "Name", scheduledTask.ComputerId);
            return View(scheduledTask);
        }

        // GET: ScheduledTasks/Edit/5
        public ActionResult EditScheduledTask(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledTask scheduledTask = db.ScheduledTasks.Find(id);
            if (scheduledTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.ComputerId = new SelectList(db.Computers, "Id", "Name", scheduledTask.ComputerId);
            return View(scheduledTask);
        }

        // POST: ScheduledTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditScheduledTask(ScheduledTask scheduledTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheduledTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = scheduledTask.ComputerId });
            }
            ViewBag.ComputerId = new SelectList(db.Computers, "Id", "Name", scheduledTask.ComputerId);
            return View(scheduledTask);
        }

        // GET: ScheduledTasks/Delete/5
        public ActionResult DeleteScheduledTask(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledTask scheduledTask = db.ScheduledTasks.Find(id);
            if (scheduledTask == null)
            {
                return HttpNotFound();
            }
            return View(scheduledTask);
        }

        // POST: ScheduledTasks/Delete/5
        [HttpPost, ActionName("DeleteScheduledTask")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteScheduledTaskConfirmed(int id)
        {
            ScheduledTask scheduledTask = db.ScheduledTasks.Find(id);
            var computerId = scheduledTask.ComputerId;
            db.ScheduledTasks.Remove(scheduledTask);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = computerId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void GenerateDropDowns(Computer computer)
        {
            ViewBag.MemoryId = new SelectList(db.Ref_Memory.OrderBy(x => x.Name), "Id", "Name", computer.MemoryId);
            ViewBag.ModelId = new SelectList(db.Ref_Model.OrderBy(x => x.Model), "ID", "Model", computer.ModelId);
            ViewBag.StatusId = new SelectList(db.Ref_Status.OrderBy(x => x.Status), "Id", "Status", computer.StatusId);
            ViewBag.VideoCardId = new SelectList(db.Ref_VideoCard.OrderBy(x => x.VideoCard), "Id", "VideoCard", computer.VideoCardId);
            ViewBag.DeviceTypeId = new SelectList(db.Ref_DeviceType.OrderBy(x => x.DeviceType), "Id", "DeviceType", computer.DeviceTypeId);
            ViewBag.ProcessorId = new SelectList(db.Ref_Processor.OrderBy(x => x.Processor), "Id", "Processor", computer.ProcessorId);
            ViewBag.SoundId = new SelectList(db.Ref_Sound.OrderBy(x => x.Sound), "Id", "Sound", computer.SoundId);
            ViewBag.OSId = new SelectList(db.Ref_OS.OrderBy(x => x.OS), "Id", "OS", computer.OSId);
        }
    }
}