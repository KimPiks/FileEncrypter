using FileEncrypter;
using FileEncrypter.Data;
using System.Runtime.InteropServices;
using Console = ConsoleFeatures.Console;

// Memory clearing method after showing password - greater security (ONLY WINDOWS!)
[DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
static extern bool ZeroMemory(IntPtr Destination, int Length);

var encryption = new Encryption();
var decryption = new Decryption();

var currentLocation = Directory.GetCurrentDirectory();
ShowFilesOptions(currentLocation);

void ShowFilesOptions(string directory)
{
    System.Console.Clear();

    var filesList = FilesList.Download(directory);

    var filesListOptions = new List<string>();
    foreach (var file in filesList)
    {
        filesListOptions.Add(file.Name);
    }

    System.Console.WriteLine(currentLocation);
    var selectedIndex = Console.Selector(filesListOptions);

    var selectedItem = filesList[selectedIndex];

    if (selectedItem.Type == FileEncrypter.Enums.FileType.Directory)
        ShowFilesOptions(selectedItem.Path);
    else
        ShowEncryptionOptions(selectedItem);
}

void ShowEncryptionOptions(FileData file)
{
    System.Console.Clear();

    System.Console.WriteLine(file.Path);

    var encryptionOptions = new List<string>();

    var parentDirectory = Path.GetDirectoryName(file.Path);
    if (parentDirectory != null)
        encryptionOptions.Add("../");

    encryptionOptions.Add("Encrypt");
    encryptionOptions.Add("Decrypt");

    var selectedIndex = Console.Selector(encryptionOptions);

    if (parentDirectory == null)
    {
        if (selectedIndex == 0)
            EncryptFile(file, "");
        else
            DecryptFile(file, "");
    }
    else
    {
        switch (selectedIndex)
        {
            case 0:
                ShowFilesOptions(parentDirectory);
                break;
            case 1:
                EncryptFile(file, parentDirectory);
                break;
            case 2:
                DecryptFile(file, parentDirectory);
                break;
        }
    }
}

void EncryptFile(FileData file, string parentDirectory)
{
    if (!FileExists(file.Path))
    {
        ShowFilesOptions(parentDirectory);
        return;
    }

    var password = encryption!.GenerateRandomPassword();
    var gch = GCHandle.Alloc(password, GCHandleType.Pinned);

    encryption.FileEncrypt(file.Path, password);
    ShowPassword(password, parentDirectory);

    ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
    gch.Free();
}

void DecryptFile(FileData file, string parentDirectory)
{
    if (!FileExists(file.Path))
    {
        ShowFilesOptions(parentDirectory);
        return;
    }

    System.Console.Clear();
    System.Console.Write("File password: ");

    var securePassword = decryption!.GetPassword();
    var password = new System.Net.NetworkCredential(string.Empty, securePassword).Password;

    var gch = GCHandle.Alloc(password, GCHandleType.Pinned);
    decryption.FileDecrypt(file.Path, file.Path.Replace(".aes", ""), password);

    ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
    gch.Free();

    Console.Acceptance("\nDecrypted. Continue?", true);
    ShowFilesOptions(parentDirectory);
}

void ShowPassword(string password, string parentDirectory)
{
    System.Console.Clear();
    System.Console.WriteLine($"Password: {password}");

    Console.Acceptance("Do you want to continue?", true);
    ShowFilesOptions(parentDirectory);
}

bool FileExists(string path) => File.Exists(path);