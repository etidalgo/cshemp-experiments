using System;
using System.Text;
using System.DirectoryServices;
using System.Security.Principal;

namespace Helpers.DirectoryServices
{
    public static class ADExtensions
    {
        /// <summary>
        /// Extension to simplify the IF statement
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string PropertyValue(this ResultPropertyCollection props, string propertyName, string defaultValue = "")
        {
            if (props.Contains(propertyName))
                return props[propertyName][0].ToString();
            return defaultValue;
        }
    }

    public class ADUtilities
    {
        public const string localDomain = "sbslocal";
        public const string localLdapGroup = "LDAP://OU=SBSGroup,DC=sbs,DC=local";

        /// <summary>
        /// Gets AD info using "WinNT://" . Has limited properties unlike LDAP://
        /// </summary>
        /// <param name="fullUserName">Expects full DOMAIN/USERNAME note the FORWARD slash</param>
        /// <returns>System.DirectoryServices.PropertyCollection </returns>
        public static string GetSid(string fullUserName)
        {
            // c# - How do I loop through a PropertyCollection - Stack Overflow <http://stackoverflow.com/questions/640792/how-do-i-loop-through-a-propertycollection>
            // DirectoryEntry.Path Property (System.DirectoryServices) <https://msdn.microsoft.com/en-us/library/system.directoryservices.directoryentry.path(v=vs.110).aspx>
            string queryString = @"WinNT://" + fullUserName;
            DirectoryEntry obDirEntry = new DirectoryEntry(queryString);
            var sid = new SecurityIdentifier((byte[])obDirEntry.Properties["objectSid"].Value, 0);
            return sid.ToString();
        }

        /// <summary>
        /// Queries the Active Directory, seeking to match the userPrincipalName.
        /// </summary>
        /// <param name="fullNetworkName">Expects entire DOMAIN\USERNAME</param>
        /// <returns></returns>
        public static SearchResult GetUserInfo(string fullNetworkName)
        {
            // adapted from Querying and Updating Active Directory Using C# (C Sharp) <http://www.ianatkinson.net/computing/adcsharp.htm>
            try
            {
                string[] tokens = fullNetworkName.Split(new Char[] { '/', '\\' });
                if (tokens.Length != 2)
                    return null; // expecting domain\username
                string userPrincipalName = String.Format("{0}@{1}", tokens[1], tokens[0]);
                string sid = GetSid(fullNetworkName.Replace(@"\", @"/"));

                // create LDAP connection object
                DirectoryEntry myLdapConnection = createDirectoryEntry();

                // create search object which operates on LDAP connection object
                // and set search object to only find the user specified
                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                search.Filter = String.Format("(objectSid={0})", sid);
                //search.Filter = "(cn=" + fullLoginName + ")";
                // search.Filter = String.Format("(sAMAccountName={0})", tokens[1]);
                //search.Filter = String.Format("(userPrincipalName={0})", userPrincipalName);

                // create results objects from search object
                return search.FindOne();
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
            }
            return null;
        }

        public static DirectoryEntry createDirectoryEntry()
        {
            // create and return new LDAP connection with desired settings

            DirectoryEntry ldapConnection = new DirectoryEntry(localDomain);
            ldapConnection.Path = localLdapGroup;
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

            return ldapConnection;
        }
    }
}

