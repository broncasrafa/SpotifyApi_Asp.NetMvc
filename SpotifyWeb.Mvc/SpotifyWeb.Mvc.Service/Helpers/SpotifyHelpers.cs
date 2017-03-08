using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Spotify.Web.Mvc.Model;
using SpotifyWeb.Mvc.Model;

namespace SpotifyWeb.Mvc.Service.Helpers
{
    public static class SpotifyHelpers
    {
        public static string FormattedBirthdate(string birthdate)
        {
            if (String.IsNullOrEmpty(birthdate))
                return "No data provided";

            DateTime date = Convert.ToDateTime(birthdate);

            string birthdateFormatted = date.ToString("MMMM dd, yyyy");

            // Retorna com a primeira letra em maiuscula
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(birthdateFormatted.ToLower());
        }

        public static string GetCountryName(string country)
        {
            if (String.IsNullOrEmpty(country))
                return "Unknow Country";

            RegionInfo rInfo = new RegionInfo(country);

            string countryName = rInfo.DisplayName;

            return countryName;
        }

        public static string TrackDurationFormatted(string duration)
        {
            int durationLength = Convert.ToInt32(duration);
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, durationLength);

            if(ts.Seconds < 10)
                return string.Format("{0}:0{1}", ts.Minutes, ts.Seconds);
            else
                return string.Format("{0}:{1}", ts.Minutes, ts.Seconds);
        }

        public static string TrackDurationFormatted_2(List<string> duration)
        {
            int durationLength = 0;
            foreach (var t in duration)
            {
                durationLength += Convert.ToInt32(t);
            }

            TimeSpan ts = new TimeSpan(0, 0, 0, 0, durationLength);

            if (ts.Minutes < 10)
                return string.Format("0{0} min", ts.Minutes);
            else
                return string.Format("{0} min", ts.Minutes);
        }

        public static void ExportToExcel(List<PlaylistTrackItem> listTracks, string filename)
        {
            List<ExportExcel> listModels = new List<ExportExcel>();

            foreach(var tck in listTracks)
            {
                ExportExcel model = new ExportExcel
                {
                    TrackName = tck.track.name,
                    AlbumName = tck.track.album.name,
                    ArtistName = tck.track.artists[0].name,
                    TrackDuration = SpotifyHelpers.TrackDurationFormatted(tck.track.duration_ms.ToString())
                };

                listModels.Add(model);
            }

            GridView gdv = new GridView();
            gdv.DataSource = listModels;
            gdv.DataBind();

            string fn = string.Format("{0}_{1}.xls", String.IsNullOrWhiteSpace(filename) ? "Arquivo" : filename, DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fn));
            System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            System.Web.HttpContext.Current.Response.Charset = "UTF-8";
            System.Web.HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            System.Web.HttpContext.Current.Response.Charset = "";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    gdv.RenderControl(htw);
                    System.Web.HttpContext.Current.Response.Output.Write(sw.ToString());
                    System.Web.HttpContext.Current.Response.Flush();
                    System.Web.HttpContext.Current.Response.End();
                }
            }

        }

        public static string FormatFollowersNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            string retorno = string.Empty;

            int tamanho = Convert.ToInt32(value.Length);
            switch (tamanho)
            {
                case 1: retorno = value; break;
                case 2: retorno = value; break;
                case 3: retorno = value; break;
                case 4: retorno = string.Format("{0}.{1}", value.Substring(0, value.Length - 3), value.Substring(1, 3)); break;
                case 5: retorno = string.Format("{0}.{1}", value.Substring(0, value.Length - 3), value.Substring(2, 3)); break;
                case 6: retorno = string.Format("{0}.{1}", value.Substring(0, value.Length - 3), value.Substring(3, 3)); break;
                case 7: retorno = string.Format("{0}.{1}.{2}", value.Substring(0, value.Length - 6), value.Substring(1, 3), value.Substring(4, 3)); break;
                case 8: retorno = string.Format("{0}.{1}.{2}", value.Substring(0, value.Length - 6), value.Substring(2, 3), value.Substring(5, 3)); break;
                case 9: retorno = string.Format("{0}.{1}.{2}", value.Substring(0, value.Length - 6), value.Substring(3, 3), value.Substring(6, 3)); break;
                case 10: retorno = string.Format("{0}.{1}.{2}.{3}", value.Substring(0, value.Length - 9), value.Substring(1, 3), value.Substring(4, 3), value.Substring(7, 3)); break;
                case 11: retorno = string.Format("{0}.{1}.{2}.{3}", value.Substring(0, value.Length - 9), value.Substring(2, 3), value.Substring(5, 3), value.Substring(8, 3)); break;
                case 12: retorno = string.Format("{0}.{1}.{2}.{3}", value.Substring(0, value.Length - 9), value.Substring(3, 3), value.Substring(6, 3), value.Substring(9, 3)); break;
                case 13: retorno = string.Format("{0}.{1}.{2}.{3}.{4}", value.Substring(0, value.Length - 12), value.Substring(1, 3), value.Substring(4, 3), value.Substring(7, 3), value.Substring(10, 3)); break;
                default:
                    retorno = "Not formatted";
                    break;
            }

            return retorno;
        }


        /// <summary>
        /// Método para separar a lista em lotes
        /// </summary>
        /// <typeparam name="T">A Classe da Lista</typeparam>
        /// <param name="list">Lista</param>
        /// <param name="parts">quantidade de lotes para separar</param>
        /// <returns>Retorna uma lista separadas em lotes pré-definidos no parâmetro</returns>
        public static IEnumerable<IEnumerable<T>> SplitList<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;

            var splits = from item in list
                         group item by i++ % parts into part
                         select part.AsEnumerable();

            return splits;
        }
    }
}
