using ManagerServer.Authentication;
using ManagerServer.Endpoints;
using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses
{
    [ProtoContract]
    internal class PostBusiness : AuthenticatedEndpoint<bool>
    {
        public string Name { get; set; }

        public override bool AuthenticatedHandle()
        {
            if (Context.GetManagerUser().Type == UserType.Restricted)
            {
                throw new ForbiddenException("Only administrators can create businesses.");
            }

            var businesses = GetApplicationData().Businesses;

            if (!businesses.IsValidName(Name))
            {
                throw new BadRequestException("Business name is missing or contains a folder separator.");
            }

            if (businesses.FileExists(Name).GetAwaiter().GetResult())
            {
                throw new BadRequestException($"Business '{Name}' already exists.");
            }

            businesses.CreateAsync(Name).GetAwaiter().GetResult();
            return true;
        }
    }
}
