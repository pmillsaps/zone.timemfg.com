using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Orchard;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Messaging.Services;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Events;
using Ventajou.ActiveDirectory.Models;

namespace Ventajou.ActiveDirectory.Services
{
    [OrchardSuppressDependency("Orchard.Users.Services.MembershipService")]
    public class MembershipService : IMembershipService {
        private readonly Orchard.Users.Services.MembershipService _baseMembershipService;
        private readonly string _defaultDomain;
        private readonly IRepository<DomainRecord> _domainsRepository;

        public MembershipService(
            IRepository<SettingsRecord> settingsRepository,
            IRepository<DomainRecord> domainsRepository,
            IOrchardServices orchardServices,
            IMessageService messageService,
            IEnumerable<IUserEventHandler> userEventHandlers,
            IClock clock,
            IEncryptionService encryptionService,
            IShapeFactory shapeFactory,
            IShapeDisplay shapeDisplay) {
            _domainsRepository = domainsRepository;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;

            _defaultDomain = settingsRepository.Table.FirstOrDefault().DefaultDomain;

            _baseMembershipService = new Orchard.Users.Services.MembershipService(orchardServices, messageService, userEventHandlers, clock, encryptionService, shapeFactory, shapeDisplay);
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        #region IMembershipService Members

        public MembershipSettings GetSettings() {
            return _baseMembershipService.GetSettings();
        }

        public IUser CreateUser(CreateUserParams createUserParams) {
            return _baseMembershipService.CreateUser(createUserParams);
        }

        public IUser GetUser(string username) {
            using (System.Web.Hosting.HostingEnvironment.Impersonate()) {
                var domainUser = ParseDomainUser(username);
                if (domainUser == null) return _baseMembershipService.GetUser(username);

                var domain = _domainsRepository.Fetch(d => d.Name == domainUser.Item1).SingleOrDefault();
                var credentialsProvided = !string.IsNullOrWhiteSpace(domain.UserName) &&
                                          !string.IsNullOrWhiteSpace(domain.Password);

                PrincipalContext context;

                // Create the context anonymously if credentials weren't supplied.
                if (!credentialsProvided)
                    context = new PrincipalContext(ContextType.Domain, domain.Name);
                else
                    context = new PrincipalContext(ContextType.Domain, domain.Name, domain.UserName, domain.Password);

                var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, domainUser.Item2);
                if (user == null) return _baseMembershipService.GetUser(username);

                string normalizedName = domainUser.Item1 + "\\" + domainUser.Item2;
                var localUser = _baseMembershipService.GetUser(normalizedName);

                if (localUser == null)
                {
                    localUser = _baseMembershipService.CreateUser(new CreateUserParams(
                        normalizedName,
                        Guid.NewGuid().ToString(),
                        user.EmailAddress,
                        @T("Active Directory User").Text,
                        Guid.NewGuid().ToString(),
                        true));
                }

                return localUser;
            }
        }

        public IUser ValidateUser(string userNameOrEmail, string password) {
            using (System.Web.Hosting.HostingEnvironment.Impersonate()) {
                var domainUser = ParseDomainUser(userNameOrEmail);
                if (domainUser == null) return _baseMembershipService.ValidateUser(userNameOrEmail, password);

                var domain = _domainsRepository.Fetch(d => d.Name == domainUser.Item1).SingleOrDefault();
                var credentialsProvided = !string.IsNullOrWhiteSpace(domain.UserName) && !string.IsNullOrWhiteSpace(domain.Password);

                PrincipalContext context;

                // Create the context anonymously if credentials weren't supplied.
                if (!credentialsProvided)
                    context = new PrincipalContext(ContextType.Domain, domain.Name);
                else
                    context = new PrincipalContext(ContextType.Domain, domain.Name, domain.UserName, domain.Password);

                if (!context.ValidateCredentials(domainUser.Item2, password)) return _baseMembershipService.ValidateUser(userNameOrEmail, password);

                // Now that the user is validated, create the context using the users credentials
                if(!credentialsProvided)
                    context = new PrincipalContext(ContextType.Domain, domain.Name, domainUser.Item2, password);

                var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, domainUser.Item2);

                string normalizedName = domainUser.Item1 + "\\" + domainUser.Item2;
                var localUser = _baseMembershipService.GetUser(normalizedName);

                if (localUser == null) {
                    localUser = _baseMembershipService.CreateUser(new CreateUserParams(
                        normalizedName,
                        Guid.NewGuid().ToString(),
                        user.EmailAddress,
                        Guid.NewGuid().ToString(),
                        @T("Active Directory User").Text,
                        true));
                }

                return localUser;
            }
        }

        public void SetPassword(IUser user, string password) {
            //HACK: probably need a better way to figure out domain membership
            if (user.UserName.Contains("\\")) throw new InvalidOperationException("You cannot change the password for a domain user");

            _baseMembershipService.SetPassword(user, password);
        }

        #endregion

        private Tuple<string, string> ParseDomainUser(string userName) {
            if (!_domainsRepository.Table.Any()) return null;

            string domainName;
            string domainUserName;

            if (userName.Contains('\\')) {
                var parts = userName.Split('\\');
                if (parts.Count() != 2) throw new ArgumentException("Invalid user name");
                domainName = parts[0];
                domainUserName = parts[1];

                if (!_domainsRepository.Fetch(d => d.Name == domainName).Any())
                    throw new ArgumentException("Unknown domain");
            }
            else if (userName.Contains('@')) {
                var parts = userName.Split('@');
                if (parts.Count() != 2) return null;
                domainName = parts[1];
                domainUserName = parts[0];

                if (!_domainsRepository.Fetch(d => d.Name == domainName).Any()) return null;
            }
            else if (!string.IsNullOrWhiteSpace(_defaultDomain)) {
                return new Tuple<string, string>(_defaultDomain, userName);
            }
            else return null;

            return new Tuple<string, string>(domainName, domainUserName);
        }
    }
}
