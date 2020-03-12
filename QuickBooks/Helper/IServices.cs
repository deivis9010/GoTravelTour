using Intuit.Ipp.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace QuickBooks.Helper
{
    public interface IServices
    {
         Task QBOApiCall(Action<ServiceContext> apiCallFunction);
    }
}
