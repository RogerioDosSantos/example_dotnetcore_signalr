using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace dotnetcore_signalr
{
    internal class ShellHub : Hub
    {
        public ShellHub()
        {
        }

        public override async Task OnConnectedAsync()
        {
            await Task.Delay(0);
            Console.WriteLine($"- Client {Context.UserIdentifier} connected the session {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Task.Delay(0);
            Console.WriteLine($"- Client {Context.UserIdentifier} disconnected the session {Context.ConnectionId}");
        }

        void ShellResponse(bool broadcast, string response)
        {
            if (string.IsNullOrEmpty(response))
                return;
            if (broadcast)
                Clients.All.SendAsync("RunShellResponse", $"{Context.ConnectionId}| {response}").GetAwaiter().GetResult();
            else
                Clients.Caller.SendAsync("RunShellResponse", $"{Context.ConnectionId}| {response}").GetAwaiter().GetResult();
        }

        public async Task RunShell(string command, string arguments, string broadcastCommand)
        {
            await Task.Delay(0);
            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine($"RunShell - Error - Command not informed");
                return;
            }
            ShellResponse(broadcastCommand == "All", $"{Context.ConnectionId}| {command} {arguments} - Started");
            using (Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = $"{command}",
                        Arguments = $"{arguments}",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                })
            {
                process.OutputDataReceived += (sender, args) => ShellResponse(broadcastCommand == "All", args.Data);
                process.ErrorDataReceived += (sender, args) => ShellResponse(broadcastCommand == "All", args.Data);
                if (!process.Start())
                {
                    Console.WriteLine($"RunShell - Error - Could not start process. Executable: {process.StartInfo.FileName} ; Arguments: {process.StartInfo.Arguments}");
                    return;
                }
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();                
                process.WaitForExit();
                ShellResponse(broadcastCommand == "All", $"{Context.ConnectionId}| {command} {arguments} - Ended (0x{process.ExitCode})");
            }
        }                 
    }
}