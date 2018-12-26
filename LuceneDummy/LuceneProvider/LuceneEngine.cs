using Lucene.Net.Analysis;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Store;
using LuceneDummy.Models;

namespace LuceneDummy.LuceneProvider
{
    public class LuceneEngine
    {
        private Analyzer _Analyzer;
        private Directory _Directory;
        private IndexWriter _IndexWriter;
        private IndexSearcher _IndexSearcher;
        private Document _Document;
        private QueryParser _QueryParser;
        private Query _Query;
        private string _IndexPath = @"C:\LuceneIndex";

        public LuceneEngine()
        {
            _Analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            _Directory = FSDirectory.Open(_IndexPath);
        }

        public void AddToIndex(IEnumerable<Classifieds> values)
        {
            using (_IndexWriter = new IndexWriter(_Directory, _Analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var loopEntity in values)
                {
                    _Document = new Document();

                    foreach (var loopProperty in loopEntity.GetType().GetProperties())
                    {
                        _Document.Add(new Field(loopProperty.Name, loopProperty.GetValue(loopEntity).ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        _IndexWriter.AddDocument(_Document);
                        _IndexWriter.Optimize();
                        _IndexWriter.Commit();
                    }
                }
            }
        }

        public List<Classifieds> Search(string field, string keyword)
        {
            _QueryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, field, _Analyzer);
            _Query = _QueryParser.Parse(keyword);

            using (_IndexSearcher = new IndexSearcher(_Directory, true))
            {
                List<Classifieds> classifieds = new List<Classifieds>();
                var result = _IndexSearcher.Search(_Query, 10);

                foreach (var loopDoc in result.ScoreDocs.OrderBy(s => s.Score))
                {
                    _Document = _IndexSearcher.Doc(loopDoc.Doc);

                    classifieds.Add(new Classifieds() { Id = System.Convert.ToInt32(_Document.Get("Id")), Title = _Document.Get("Title"), Price = Convert.ToDouble(_Document.Get("Price")), Region = _Document.Get("Region"), City = _Document.Get("City"), CityArea = _Document.Get("CityArea") });
                }

                return classifieds;
            }
        }
    }
}
