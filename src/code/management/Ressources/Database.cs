using LiteDB;
using System.Linq.Expressions;

namespace Uniray
{
    /// <summary>Represents an instance of a <see cref="DatabaseConnection"/> object using LightDB.</summary>
    public class DatabaseConnection
    {
        private readonly string _file;

        private readonly LiteDatabase _database;

        private readonly Dictionary<string, object> _collectionsCache;

        private readonly List<string> _collectionNames;

        /// <summary>Creates a <see cref="DatabaseConnection"/> instance.</summary>
        /// <param name="path"></param>
        public DatabaseConnection(string path)
        {
            try
            {
                _file = path;
                _database = new LiteDatabase(_file);
                _collectionsCache = new Dictionary<string, object>();
                _collectionNames = _database.GetCollectionNames().ToList();
                // Display message
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("INFO: Database connection established successfully!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch
            {
                throw new Exception($"Unable to locate database file {path}");
            }
        }

        /// <summary>Creates a reference to a collection if one not already exists in cache.</summary>
        /// <typeparam name="T">Type of the collections's documents.</typeparam>
        /// <param name="collection">Name of the collection.</param>
        /// <returns>The newly created or not collection.</returns>
        private ILiteCollection<T> GetOrCreateCollection<T>(string collection)
        {
            if (_collectionsCache.ContainsKey(collection))
            {
                return (ILiteCollection<T>)_collectionsCache[collection];
            }
            else
            {
                if (!_collectionNames.Contains(collection)) throw new Exception("Collection does not exist.");
                _collectionsCache.Add(collection, _database.GetCollection<T>(collection));
                return (ILiteCollection<T>)_collectionsCache[collection];
            }
        }

        // ----------------------------------------------------------------
        // Select functions
        // ----------------------------------------------------------------

        /// <summary>Selects every document satisfying a condition in a single collection.</summary>
        /// <typeparam name="T">Type of the collections's documents.</typeparam>
        /// <param name="collection">Collection to use.</param>
        /// <param name="predicate">Predicate to use for document search.</param>
        /// <returns><see cref="List{T}"/> containing all the selected documents.</returns>
        public List<T> Select<T>(string collection, Expression<Func<T, bool>> predicate)
        {
            return GetOrCreateCollection<T>(collection).Find(predicate).ToList();
        }

        /// <summary>Selects every document in a single collection.</summary>
        /// <typeparam name="T">Type of the collection's documents.</typeparam>
        /// <param name="collection">Collection to use.</param>
        /// <returns><see cref="List{T}"/> containing all the collection's documents.</returns>
        public List<T> Select<T>(string collection)
        {
            return GetOrCreateCollection<T>(collection).FindAll().ToList();
        }

        // ----------------------------------------------------------------
        // Insert functions
        // ----------------------------------------------------------------

        /// <summary>Inserts a document in a single collection</summary>
        /// <typeparam name="T">Type of the collection's documents.</typeparam>
        /// <param name="collection">Collection to use.</param>
        /// <param name="documents">Document to insert.</param>
        public void Insert<T>(string collection, T document)
        {
            GetOrCreateCollection<T>(collection).Insert(document);
        }

        /// <summary>Inserts a list of documents in a single collection</summary>
        /// <typeparam name="T">Type of the collection's documents.</typeparam>
        /// <param name="collection">Collection to use.</param>
        /// <param name="documents">Documents to insert.</param>
        public void Insert<T>(string collection, List<T> documents)
        {
            GetOrCreateCollection<T>(collection).InsertBulk(documents);
        }

        // ----------------------------------------------------------------
        // Delete functions
        // ----------------------------------------------------------------

        /// <summary>Deletes every document of a single collection.</summary>
        /// <typeparam name="T">Type of the collection's documents.</typeparam>
        /// <param name="collection">Collection to use.</param>
        /// <param name="predicate">Predicate to use for document search.</param>
        public void Delete<T>(string collection, Expression<Func<T, bool>> predicate)
        {
            GetOrCreateCollection<T>(collection).DeleteMany(predicate);
        }

        /// <summary>Deletes every document of a single collection.</summary>
        /// <typeparam name="T">Type of the collection's documents.</typeparam>
        /// <param name="collection">Collection to use.</param>
        public void Delete<T>(string collection)
        {
            GetOrCreateCollection<T>(collection).DeleteAll();
        }

        // ----------------------------------------------------------------
        // Update functions
        // ----------------------------------------------------------------

        /// <summary>Updates every document satisfying a condition in a single collection.</summary>
        /// <typeparam name="T">Type of the collection's documents.</typeparam> 
        /// <param name="collection">Collection to use.</param>
        /// <param name="document">Document to update.</param>
        /// <param name="predicate">Predicate to use for document search.</param>
        public void Update<T>(string collection, Expression<Func<T, T>> expression, Expression<Func<T, bool>> predicate)
        {
            GetOrCreateCollection<T>(collection).UpdateMany(expression, predicate);
        }

        // ----------------------------------------------------------------
        // Other functions
        // ----------------------------------------------------------------

        /// <summary>Counts the documents of a specific collection.</summary>
        /// <typeparam name="T">Type of the collection's documents.</typeparam>
        /// <param name="collection">Collection to count.</param>
        /// <returns>The total number of documents in the collection.</returns>
        public int CountCollection<T>(string collection)
        {
            return GetOrCreateCollection<T>(collection).Count();
        }
    }
}
