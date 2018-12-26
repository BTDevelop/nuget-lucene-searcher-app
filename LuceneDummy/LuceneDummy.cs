using LuceneDummy.LuceneProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using LuceneDummy.Models;
using Newtonsoft.Json;

namespace LuceneDummy
{
    public class LuceneDummy
    {
        public string DummySearcher(string wanted)
        {
            LuceneEngine engine = new LuceneEngine();
            var result = engine.Search("Title", wanted);
            return JsonConvert.SerializeObject(result);
        }

        public void DummyCreator()
        {
            LuceneEngine engine = new LuceneEngine();
            var classifieds = Builder<Classifieds>.CreateListOfSize(1000000)
                .All()
                .With(c => c.Id = Faker.RandomNumber.Next(1000000))
                .With(c => c.Title = Faker.Lorem.Sentence(6))
                .With(c => c.Price = Faker.RandomNumber.Next(1000000))
                .With(c => c.Region = Faker.Lorem.Sentence(1))
                .With(c => c.City = Faker.Lorem.Sentence(1))
                .With(c => c.CityArea = Faker.Lorem.Sentence(1))
                .Build();

            engine.AddToIndex(classifieds);
        }
    }
}
