using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business
{
    internal interface IView
    {
        string Business { get; set; }
        Guid? Key { get; set; }
        string Referrer { get; set; }
        string Language { get; set; }
        HttpContext Context { get; set; }
        public View GetView();
    }
}
