// Re: How to get user's full name using HttpContext.Current.User | PC Review <http://www.pcreview.co.uk/threads/re-how-to-get-users-full-name-using-httpcontext-current-user.1277474/>
// csc /debug /pdb:who who.cs

using System;
using System.DirectoryServices;
using System.Collections;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
					
public class Program
{
    // c# - How do I loop through a PropertyCollection - Stack Overflow <http://stackoverflow.com/questions/640792/how-do-i-loop-through-a-propertycollection>
    private static void DisplayInfo( string queryString )
    {
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
    }

    private static void Something()
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
	public static void Main()
	{
		string Domain_Slash_User = @"SBSLocal\ETIDALGO";
		string userName = Domain_Slash_User.Replace(@"\", @"/");
		string queryString = @"WinNT://" + userName;
        DisplayInfo(queryString);

        // DisplayInfo(@"LDAP://CN=Ernest Tidalgo,OU=SBS,OU=AKL,OU=NORTH,OU=SBSGroup,DC=sbs,DC=local,<default domain> [chjwdc1.sbs.local]");
        DisplayInfo(@"(CN=Ernest Tidalgo)");

	}
}
