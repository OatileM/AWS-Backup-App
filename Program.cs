using System;
using System.IO;
using System.IO.Compression;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        // AWS credentials and S3 bucket details
        var accessKey = "YOUR ACCESS KEY";
        var secretKey = "YOUR SECRET KEY";
        var bucketName = "YOUR BUCKET NAME";

        // Local folder path for backup
        var localFolderPath = "YOUR PATH";

        // Create a temporary zip file
        var zipFilePath = Path.GetTempFileName() + ".zip";
        ZipFile.CreateFromDirectory(localFolderPath, zipFilePath);

        // Create an S3 client
        var s3Client = new AmazonS3Client(accessKey, secretKey,  RegionEndpoint.AFSouth1);

        // Specify a unique key for the zip file in the S3 bucket
        var s3Key = $"backup-{DateTime.Now:yyyyMMdd-HHmmss}.zip";

        try
        {
            // Upload the zip file to S3
            await UploadFileToS3(s3Client, bucketName, s3Key, zipFilePath);

            Console.WriteLine("Folder backup successfully uploaded to S3.");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // Delete the temporary zip file
            File.Delete(zipFilePath);
        }
    }

    static async System.Threading.Tasks.Task UploadFileToS3(IAmazonS3 s3Client, string bucketName, string key, string filePath)
    {
        var fileTransferUtility = new TransferUtility(s3Client);

        // Set the CannedACL (Access Control List) for the S3 object - here, it's set to Private
        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
        {
            BucketName = bucketName,
            Key = key,
            FilePath = filePath,
            CannedACL = S3CannedACL.Private
        };

        await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
    }



}

