﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dtx = new DataClasses1DataContext();

            var url = "https://turbo.az/autos";
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);

            IList<HtmlNode> nodes = doc.QuerySelectorAll("div.products")[2]
            .QuerySelectorAll("div.products-i");

            var data = nodes.Select(node =>
            {
                var name = node.QuerySelector("a.products-i__link").Attributes["href"].Value;

                return new
                {
                    name = name
                };
            });


            List<string> urls = new List<string>();
            foreach (var item in data)
            {
                urls.Add(item.name);
            }
            List<string> urls2 = new List<string>();

            HtmlWeb web1 = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc1;
            IList<HtmlNode> nodes1;
            foreach (var linkurl in urls)
            {
                var orginalurl = "https://turbo.az/" + linkurl;
                doc1 = web1.Load(orginalurl);
                nodes1 = doc1.QuerySelectorAll("div.product-properties-container").QuerySelectorAll("ul.product-properties li.product-properties-i").ToList();
                var properties = nodes1.Select(node =>
                {
                    var name = node.QuerySelector("div.product-properties-value").InnerText;
                    return new
                    {
                        name = name,
                    };
                });

                foreach (var item in properties)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("**************");
                dtx.Cars.InsertOnSubmit(
                  new Car
                  {
                      Mileage = int.Parse(properties.ToList()[9].name.Remove(properties.ToList()[9].name.IndexOf("km")).Trim().Replace(" ", null)),
                      RegYear = int.Parse(properties.ToList()[3].name),
                      Price = decimal.Parse(properties.ToList()[13].name.Split('A','$')[0].Trim().Replace(" ", null)),
                      MakeId = dtx.Makes.FirstOrDefault(x => x.Name == properties.ToList()[1].name).Id,
                      ModelId = dtx.Models.FirstOrDefault(x => x.Name == properties.ToList()[2].name).Id,
                      ColorId = dtx.Colors.FirstOrDefault(x => x.Name == properties.ToList()[5].name).Id,
                      RegionId = dtx.Regions.FirstOrDefault(x => x.Name == properties.ToList()[0].name).Id,
                      BanTypeId = dtx.BanTypes.FirstOrDefault(x => x.Name == properties.ToList()[4].name).Id,
                      GearId = dtx.Gears.FirstOrDefault(x => x.Name == properties.ToList()[11].name).Id,
                      FuelTypeId = dtx.FuelTypes.FirstOrDefault(x => x.Name == properties.ToList()[8].name).Id,
                      TransmissionId = dtx.Transmissions.FirstOrDefault(x => x.Name == properties.ToList()[10].name).Id,
                      EngVolumeId = dtx.EngineVolumes.FirstOrDefault(x => x.Volume == double.Parse(properties.ToList()[6].name.Remove(properties.ToList()[6].name.IndexOf('L')).Trim()) * 100).Id
                  });
                dtx.SubmitChanges();

            }

            //0 -> Seher
            //1 -> Marka
            //2 -> Model
            //3 -> regYear
            //4 -> ban type
            //5 -> color
            //6 -> eng. volume
            //8 -> fuel type
            //9 -> yurus
            //10 -> suretler qutusu
            //11 -> oturucu
            //13 -> qiymet
            Console.WriteLine("Success");
            Console.Read();

        }
    }
}
