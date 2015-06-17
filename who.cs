// Re: How to get user's full name using HttpContext.Current.User | PC Review <http://www.pcreview.co.uk/threads/re-how-to-get-users-full-name-using-httpcontext-current-user.1277474/>

using System;
using System.DirectoryServices;
					
public class Program
{
	public static void Main()
	{
		string Domain_Slash_User = @"SBSLocal\ETIDALGO";
		string Domain_Slash_Machine = Domain_Slash_User.Replace(@"\", @"/");
		string queryString = @"WinNT://" + Domain_Slash_Machine;

		DirectoryEntry obDirEntry = new DirectoryEntry(queryString);
		System.DirectoryServices.PropertyCollection coll = obDirEntry.Properties;

		object obVal = coll["FullName"].Value;
		string _User = obVal.ToString();
		Console.WriteLine(_User);
	}
}
