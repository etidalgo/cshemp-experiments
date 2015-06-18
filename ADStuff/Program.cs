using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Collections;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;

namespace ADStuff
{
    // Re: How to get user's full name using HttpContext.Current.User | PC Review <http://www.pcreview.co.uk/threads/re-how-to-get-users-full-name-using-httpcontext-current-user.1277474/>
    // csc /debug /pdb:who who.cs

    class Program
    {
        // c# - How do I loop through a PropertyCollection - Stack Overflow <http://stackoverflow.com/questions/640792/how-do-i-loop-through-a-propertycollection>
        private static void DisplayInfo(string userName)
        {
            string queryString = @"WinNT://" + userName; 
            DirectoryEntry obDirEntry = new DirectoryEntry(queryString);
            System.DirectoryServices.PropertyCollection coll = obDirEntry.Properties;
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
            // Console.WriteLine(coll["StreetAddress"].Value.ToString());
        }

        private static void GetAllEntries() // MESSY!
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

        static void GetAUserPrincipal(String username)
        {
            try  
            {  
                // enter AD settings  
                PrincipalContext AD = new PrincipalContext(ContextType.Domain, "sbs.local");  
  
                // create search user and add criteria  
                UserPrincipal u = new UserPrincipal(AD);
                u.SamAccountName = username;
  
                // search for user  
                PrincipalSearcher search = new PrincipalSearcher(u);  
                UserPrincipal result = (UserPrincipal)search.FindOne();
                search.Dispose();  
  
                // show some details  
                Console.WriteLine("Given Name : " + result.GivenName);  

                Console.WriteLine("Display Name : " + result.DisplayName);  
                Console.WriteLine("Phone Number : " + result.VoiceTelephoneNumber);  
            }  
  
            catch (Exception e)  
            {  
                Console.WriteLine("Error: " + e.Message);  
            }  

        }
        public static void Main()
        {
            string fullNetworkName = @"SBSLocal\ETIDALGO";
            string domainSlashMachine = fullNetworkName.Replace(@"\", @"/");

            DisplayInfo(domainSlashMachine);

            string[] names = domainSlashMachine.Split(new Char[] { '/', '\\' });
            string userName = names[names.Count() - 1];

            // GetAllEntries();
            GetAUserPrincipal(userName);

            // Console.Write("Enter user (firstname surname): ");
            // String fullName = Console.ReadLine();

            DirServices.GetUserInfo(userName);
        }
    }
}
