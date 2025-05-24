using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using MurshadikCP.Models.ViewModels;

namespace MurshadikCP.Interface
{
    interface IVideoBoard
    {
        List<VideoViewModelForDashboard> GetGallery(VideoSearchViewModel videoSearch);
    }
}
