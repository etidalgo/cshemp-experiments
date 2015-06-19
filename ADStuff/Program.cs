using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Collections;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using Helpers.DirectoryServices;

namespace ADStuff
{
    // Re: How to get user's full name using HttpContext.Current.User | PC Review <http://www.pcreview.co.uk/threads/re-how-to-get-users-full-name-using-httpcontext-current-user.1277474/>
    // csc /debug /pdb:who who.cs

    class Program
    {
        /// <summary>
        /// Gets AD info using "WinNT://" . Has limited properties unlike LDAP://
        /// </summary>
        /// <param name="fullUserName">Expects full DOMAIN/USERNAME note the FORWARD slash</param>
        /// <returns>System.DirectoryServices.PropertyCollection </returns>
        public static System.DirectoryServices.PropertyCollection DisplayInfo(string fullUserName)
        {
            // c# - How do I loop through a PropertyCollection - Stack Overflow <http://stackoverflow.com/questions/640792/how-do-i-loop-through-a-propertycollection>
            // DirectoryEntry.Path Property (System.DirectoryServices) <https://msdn.microsoft.com/en-us/library/system.directoryservices.directoryentry.path(v=vs.110).aspx>
            string queryString = @"WinNT://" + fullUserName; 
            DirectoryEntry obDirEntry = new DirectoryEntry(queryString);
            return obDirEntry.Properties;
        }

        private static void GetAllEntries() // overkill, gets everything...everything
        {
            using (var context = new PrincipalContext(ContextType.Domain, "sbs.local"))
            {
                using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    foreach (var result in searcher.FindAll())
                    {
                        DirectoryEntry entry = result.GetUnderlyingObject() as DirectoryEntry;

                        foreach (string key in entry.Properties.PropertyNames)
                        {

                            string sPropertyValues = String.Empty;

                            foreach (object pc in entry.Properties[key])
                            {

                                sPropertyValues += Convert.ToString(pc) + ";";

                            }

                            sPropertyValues = sPropertyValues.Substring(0, sPropertyValues.Length - 1);

                            Console.WriteLine(key + "=" + sPropertyValues);
                        }

                        Console.WriteLine("=======================================");
                    }

                    Console.ReadKey();
                }
            }
        }

        // Querying and Updating Active Directory Using C# (C Sharp) <http://www.ianatkinson.net/computing/adcsharp.htm#ex6>
        // UserPrincipal Class (System.DirectoryServices.AccountManagement) <https://msdn.microsoft.com/en-us/library/system.directoryservices.accountmanagement.userprincipal(v=vs.110).aspx>

        static void GetAUserPrincipal(String fullNetworkName)
        {
            try  
            {
                string[] tokens = fullNetworkName.Split(new Char[] { '/', '\\' });
                if (tokens.Length != 2)
                    return ; // expecting domain\username
                string userPrincipalName = String.Format("{0}@{1}", tokens[1], tokens[0]);
                
                // enter AD settings  
                PrincipalContext AD = new PrincipalContext(ContextType.Domain, tokens[0]);  
  
                // create search user and add criteria  
                UserPrincipal u = new UserPrincipal(AD);
                u.SamAccountName = tokens[1];
  
                // search for user  
                PrincipalSearcher search = new PrincipalSearcher(u);  
                UserPrincipal result = (UserPrincipal)search.FindOne();
                search.Dispose();  
  
                // show some details  
                Console.WriteLine("Given Name : " + result.GivenName);  

                Console.WriteLine("Display Name : " + result.DisplayName);  
                Console.WriteLine("Phone Number : " + result.VoiceTelephoneNumber);
                Console.WriteLine("DistinguishedName : " + result.DistinguishedName);
                Console.WriteLine("Sid : " + result.Sid);

                Console.WriteLine("UserCannotChangePassword : " + result.UserCannotChangePassword);
                Console.WriteLine("UserPrincipalName : " + result.UserPrincipalName);
            }  
  
            catch (Exception e)  
            {  
                Console.WriteLine("Error: " + e.Message);  
            }  

        }
        public static void Main()
        {
            Console.WriteLine();
            string fullNetworkName = @"SBSLocal\ETIDALGO";
            //string fullNetworkName = @"SBS312002\Julius";
            //string fullNetworkName = @"SBS312002\ETIDALGO";
            //string fullNetworkName = @"SBS312002\PASHCROFT";
            string domainSlashMachine = fullNetworkName.Replace(@"\", @"/");

            Console.WriteLine(" ---------------------------------------------------------- Using DirectoryEntry");
            System.DirectoryServices.PropertyCollection coll = DisplayInfo(domainSlashMachine);
            if (coll != null)
            {
                IDictionaryEnumerator ide = coll.GetEnumerator();
                ide.Reset();
                while (ide.MoveNext())
                {
                    PropertyValueCollection pvc = ide.Entry.Value as PropertyValueCollection;

                    Console.WriteLine("{0} : {1}", ide.Entry.Key.ToString(), pvc.Value);
                }

                // Debugger.Break();
                object obVal = coll["FullName"].Value;

                string _User = obVal.ToString();
                Console.WriteLine(_User);
                var sid = new SecurityIdentifier((byte[])coll["objectSid"].Value, 0);
                Console.WriteLine(sid.ToString());
                // Console.WriteLine(coll["StreetAddress"].Value.ToString());
            }
            // ------------------------------

            string[] names = domainSlashMachine.Split(new Char[] { '/', '\\' });
            string userName = names[names.Count() - 1];


            // GetAllEntries();
            Console.WriteLine(" ----------------------------------------------------- Using PrincipalContext");

            GetAUserPrincipal(fullNetworkName);

            // Console.Write("Enter user (firstname surname): ");
            // String fullName = Console.ReadLine();

            // ------------------------------
            Console.WriteLine(" ----------------------------------------------------- Using DirectorySearcher");

            SearchResult userInfo = ADUtilities.GetUserInfo(fullNetworkName);
            if (userInfo != null)
            {
                // user exists, cycle through LDAP fields (cn, telephonenumber etc.)
                ResultPropertyCollection fields = userInfo.Properties;

                foreach (String ldapField in fields.PropertyNames)
                {
                    // cycle through objects in each field e.g. group membership
                    // (for many fields there will only be one object such as name)

                    foreach (Object myCollection in fields[ldapField])
                        Console.WriteLine(String.Format("{0,-20} : {1}", ldapField, myCollection.ToString()));

                }

                Console.WriteLine();
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

                for (int i = 0; i < pairs.GetLength(0); i++)
                {
                    // for most fields there will be only one object, ie fields["displayname"][0]
                    if (fields.Contains(pairs[i, 0]))
                        Console.WriteLine("{0}: {1}", pairs[i, 1], fields[pairs[i, 0]][0].ToString()); // uglyuglyugly
                }

                // alternatively
                Console.WriteLine(" ---------------------------------------------------------- Using extensions");
                for (int i = 0; i < pairs.GetLength(0); i++)
                {
                    Console.WriteLine("{0}: {1}", pairs[i, 1], fields.PropertyValue(pairs[i, 0], "[Not found ]" ));
                }


            }
            else
            {
                // user does not exist
                Console.WriteLine("User ({0}) not found!", fullNetworkName);
            }

        }
    }
}
