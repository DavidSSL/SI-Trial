using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using SimpleInjector;

namespace SI_Trial
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            var container = new Container();

            Bootstrap.Start(container, RegionEndpoint.AFSouth1);

            var fileNamesGetter = container.GetInstance<IGetFileNames>();
            var uidMappingFileNames =
                fileNamesGetter.GetAsync(S3ClientType.Discriminator2, "bucketName");
        }
    }
    
    internal static class Bootstrap
    {
        public static void Start(Container container, RegionEndpoint bucketRegion)
        {
            container.Register<IGetFileNames, S3FileNamesGetter>();
            container.Options.EnableAutoVerification = false;

            container.Verify();
        }
    } 
    public enum S3ClientType
    {
        Discriminator1,
        Discriminator2,
    }
    
    public interface IGetFileNames
    {
        IAsyncEnumerable<string> GetAsync(S3ClientType discriminator, string bucketName);
    }

    public class S3FileNamesGetter: IGetFileNames
    {
        private readonly IDictionary<S3ClientType, IAmAnS3ClientQueryor> s3Clients;

        public S3FileNamesGetter(IDictionary<S3ClientType, IAmAnS3ClientQueryor> s3Clients)
        {
            this.s3Clients = s3Clients;
        }

        public IAsyncEnumerable<string> GetAsync(S3ClientType type, string bucketName)
        {
            return s3Clients[type].GetFileNames(bucketName);
        }
    }

    public interface IAmAnS3ClientQueryor
    {
        IAsyncEnumerable<string> GetFileNames(string bucketName);
    }

    public class S3Queryor : IAmAnS3ClientQueryor
    {
        private readonly IAmazonS3 s3Client;

        public S3Queryor(IAmazonS3 s3Client)
        {
            this.s3Client = s3Client;
        }

        public async IAsyncEnumerable<string> GetFileNames(string bucketName)
        {
            yield return "";
        }
    }
}
