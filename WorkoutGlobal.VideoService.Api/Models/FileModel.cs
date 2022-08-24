using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WorkoutGlobal.VideoService.Api.Contracts;

namespace WorkoutGlobal.VideoService.Api.Models
{
    public class FileModel : IModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("length")]
        public int Length { get; set; }

        [BsonElement("сhunkSize")]
        public int ChunkSize { get; set; }

        [BsonElement("uploadDate")]
        public DateTime UploadDate { get; set; }

        [BsonElement("md5")]
        public string MD5 { get; set; }

        [BsonElement("fileName")]
        public string FileName { get; set; }
    }
}
