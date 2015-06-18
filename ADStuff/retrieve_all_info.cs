using System;
using System.Text;
using System.DirectoryServices;

namespace ADStuff
{
    class DirServices
    {
        public static void GetUserInfo(String username)
        {
            try
            {
                // create LDAP connection object

                DirectoryEntry myLdapConnection = createDirectoryEntry();

                // create search object which operates on LDAP connection object
                // and set search object to only find the user specified

                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                //search.Filter = "(cn=" + username + ")";
                search.Filter = "(sAMAccountName=" + username + ")";

                // create results objects from search object

                SearchResult result = search.FindOne();
                
                if (result != null)
                {
                    // user exists, cycle through LDAP fields (cn, telephonenumber etc.)

                    ResultPropertyCollection fields = result.Properties;

                    foreach (String ldapField in fields.PropertyNames)
                    {
                        // cycle through objects in each field e.g. group membership
                        // (for many fields there will only be one object such as name)

                        foreach (Object myCollection in fields[ldapField]) 
                            Console.WriteLine(String.Format("{0,-20} : {1}", ldapField, myCollection.ToString()));
                               
                    }

                    // Arrays Tutorial (C#) <https://msdn.microsoft.com/en-us/library/aa288453(v=vs.71).aspx>
                    string[,] pairs = new string[,] { 
                        { "displayName", "Fool Name" }, 
                        { "name", "Name" },
                        { "streetAddress", "Street Address"},
                        {"mail", "Mail"},
                        {"telephoneNumber", "Phone"},
                        {"freezingpoint", "Freezing Point"},
                        {"sAMAccountName", "sAMAccountName"},
                        {"title", "Title"},
                        {"department", "Department"}
                    };

                    for (int i = 0; i < pairs.GetLength(0); i++ )
                    {
                        // for most fields there will be only one objects
                        if (fields.Contains(pairs[i, 0])) 
                            Console.WriteLine("{0}: {1}", pairs[i, 1], fields[pairs[i,0]][0].ToString()); // uglyuglyugly
                    }
                    // if (fields.Contains("displayname")) Console.WriteLine("Fool Name: {0}", fields["displayname"][0].ToString());

                }

                else
                {
                    // user does not exist
                    Console.WriteLine("User ({0}) not found!", username);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
            }
        }

        public static DirectoryEntry createDirectoryEntry()
        {
            // create and return new LDAP connection with desired settings

            DirectoryEntry ldapConnection     = new DirectoryEntry("sbs.local");
            ldapConnection.Path               = "LDAP://OU=SBSGroup,DC=sbs,DC=local";
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

            return ldapConnection;
        }
    }
}
