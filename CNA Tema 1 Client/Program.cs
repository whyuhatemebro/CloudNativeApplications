using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Tema_1_Client.Protos;

namespace Tema_1_Client
{
    class Program
    {
        static public NonProcessedUserData ReadUserDataFromConsole()
        {
            Console.WriteLine("Name:");
            string name = Console.ReadLine();
            Console.WriteLine("CNP:");
            string cnp = Console.ReadLine();
            NonProcessedUserData nonProcessedUserData = new NonProcessedUserData
            {
                Name = name,
                Cnp = cnp
            };
            return nonProcessedUserData;
        }
        static async Task Main(string[] args)
        {
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new UserDataProcessingService.UserDataProcessingServiceClient(channel);
            SendUserDataResponse response = await client.SendUserDataAsync(new SendUserDataRequest() { NonProcessedUserData = ReadUserDataFromConsole() });
            ProcessedUserData processedUserData = response.ProcessedUserData;
            Console.WriteLine($"Hello User:{processedUserData.Name},with Age:{processedUserData.Age} and Gender:{processedUserData.Gender}");
        }
    }
}
