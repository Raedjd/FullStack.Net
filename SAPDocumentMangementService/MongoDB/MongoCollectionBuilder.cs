using MongoDB.Driver;

namespace SAPDocumenMangementService.MongoDB
{
    public class MongoCollectionBuilder
    {
        public static IMongoCollection<E> GetMongoCollection<E>(IMongoDatabase database)
        {
            var collection = database.GetCollection<E>(GetCollectionName<E>());
            return collection;
        }

        public static string GetCollectionName<E>()
        {
            string collectionname;

            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = Attribute.GetCustomAttribute(typeof(E), typeof(CollectionName));
            if (att != null)
            {
                // It does! Return the value specified by the CollectionName attribute
                collectionname = ((CollectionName)att).Name;
            }
            else
            {
                collectionname = typeof(E).Name;
            }

            return collectionname;
        }
    }
}
