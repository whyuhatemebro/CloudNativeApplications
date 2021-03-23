using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tema_1_Server.Protos;

namespace Tema_1_Server.Services
{
    public class UserDataProcessingService : Tema_1_Server.Protos.UserDataProcessingService.UserDataProcessingServiceBase
    {
        public int CalculateAgeFromBirthday(DateTime birthDay)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthDay.Year;

            if (now.Month < birthDay.Month || (now.Month == birthDay.Month && now.Day < birthDay.Day))
                age--;

            return age;
        }

        public DateTime GetBirthdayFromCNP(string cnp)
        {
            DateTime birthday = DateTime.Now;
            int year = Int32.Parse(cnp.Substring(1, 2));
            if(cnp.ElementAt(0) == '1' || cnp.ElementAt(0) == '2')
            {
                year += 1900;
            }
            else if(cnp.ElementAt(0) == '3' || cnp.ElementAt(0) == '4')
            {
                year += 1800;
            }
            else if (cnp.ElementAt(0) == '5' || cnp.ElementAt(0) == '6')
            {
                year += 2000;
            }
            int month = Int32.Parse(cnp.Substring(3, 2));
            int day = Int32.Parse(cnp.Substring(5, 2));
            if(DateTime.TryParse($"{month}/{day}/{year}", out birthday))
            {
                return birthday;
            }
            else
            {
                throw new Exception("Invalid CNP");
            }
        }
        
        public ProcessedUserData.Types.Gender GetGenderFromCNP(string cnp)
        {
            if(Int32.Parse(cnp.ElementAt(0).ToString())%2 == 0)
            {
                return ProcessedUserData.Types.Gender.Female;
            }
            else
            {
                return ProcessedUserData.Types.Gender.Male;
            }
        }
        public ProcessedUserData ProcessUserData(NonProcessedUserData nonProcessedUserData)
        {
            Console.WriteLine("Processing data...");
            if (nonProcessedUserData.Cnp.Length != 13)
            {
                throw new Exception("Invalid CNP length.");
            }
            DateTime birthday = DateTime.Now;
            string age;
            try
            {
                birthday = GetBirthdayFromCNP(nonProcessedUserData.Cnp);
                age = CalculateAgeFromBirthday(birthday).ToString();
            }
            catch(Exception error) 
            {
                throw error;
            }
             
            ProcessedUserData.Types.Gender gender = GetGenderFromCNP(nonProcessedUserData.Cnp);
            return new ProcessedUserData
            {
                Name = nonProcessedUserData.Name,
                Age = age,
                Gender = gender
            };
        }

        public override Task<SendUserDataResponse> SendUserData(SendUserDataRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Request from User:{request.NonProcessedUserData.Name},with CNP:{request.NonProcessedUserData.Cnp}");
            ProcessedUserData processedUserData = new ProcessedUserData();
            try
            {
                processedUserData = ProcessUserData(request.NonProcessedUserData);
                Console.WriteLine($"Processed data User:{processedUserData.Name},with Age:{processedUserData.Age} and Gender:{processedUserData.Gender}");
            }
            catch(Exception error)
            {
                processedUserData.Name = request.NonProcessedUserData.Name;
                processedUserData.Age = error.Message;
                Console.WriteLine(error.Message);
            }
           
            SendUserDataResponse sendUserDataResponse = new SendUserDataResponse()
            {
                ProcessedUserData = processedUserData
            };
            return Task.FromResult(sendUserDataResponse);
        }
    }
}
