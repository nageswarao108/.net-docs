using Azure.Storage.Blobs;
using Microsoft.Net.Http.Headers;            // for app.MapGet("/stream-video/{blobName}/{containerName}"
#region snippet
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/process-image/{strImage}", (string strImage, HttpContext http, CancellationToken token) =>
{
    http.Response.Headers.CacheControl = $"public,max-age={TimeSpan.FromHours(24).TotalSeconds}";
    return Results.Stream(stream => Resize(strImage, stream, token), "image/jpeg");
});

async Task Resize(string strImage, Stream stream, CancellationToken token)
{
    var strPath = $"wwwroot/img/{strImage}";
    using var image = await Image.LoadAsync(strPath, token);
    int width = image.Width / 2;
    int height = image.Height / 2;
    image.Mutate(x =>x.Resize(width, height)
    );
    await image.SaveAsync(stream, JpegFormat.Instance, cancellationToken: token);
}
#endregion

app.MapGet("/rotate-image/{strImage}", (string strImage, HttpContext http, CancellationToken token) =>
{
    http.Response.Headers.CacheControl = $"public,max-age={TimeSpan.FromHours(24).TotalSeconds}";
    return Results.Stream(stream => RotateImage(strImage, stream, token), "image/jpeg");
});

async Task RotateImage(string strImage, Stream stream, CancellationToken token)
{
    var strPath = $"wwwroot/img/{strImage}";
    using var image = await Image.LoadAsync(strPath, token);
    int width = image.Width / 2;
    int height = image.Height / 2;
    image.Mutate(x => x.Rotate(RotateMode.Rotate90)
    );
    await image.SaveAsync(stream, JpegFormat.Instance, cancellationToken: token);
}

#region snippet_abs
app.MapGet("/stream-image/{blobName}/{containerName}", 
    async (string blobName, string containerName, CancellationToken token) =>
{
    var conStr = builder.Configuration["blogConStr"];
    BlobContainerClient blobContainerClient = new BlobContainerClient(conStr, containerName);
    BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
    return Results.Stream(await blobClient.OpenReadAsync(cancellationToken: token), "image/jpeg");
});
#endregion

#region snippet_video
// GET /stream-video/earth.mp4/videos
app.MapGet("/stream-video/{blobName}/{containerName}",
    async (HttpContext http, CancellationToken token, string blobName, string containerName) =>
{
    var conStr = builder.Configuration["blogConStr"];
    BlobContainerClient blobContainerClient = new BlobContainerClient(conStr, containerName);
    BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
    
    var properties = await blobClient.GetPropertiesAsync(cancellationToken: token);
    
    DateTimeOffset lastModified = properties.Value.LastModified;
    long length = properties.Value.ContentLength;
    
    long etagHash = lastModified.ToFileTime() ^ length;
    var entityTag = new EntityTagHeaderValue('\"' + Convert.ToString(etagHash, 16) + '\"');
    
    http.Response.Headers.CacheControl = "public,max-age=86400";

    return Results.Stream(await blobClient.OpenReadAsync(cancellationToken: token), 
        contentType: "video/mp4",
        lastModified: lastModified,
        entityTag: entityTag,
        enableRangeProcessing: true);
});
#endregion

app.MapGet("/", () => "Blob test");

// quick test get an image and return it
app.MapGet("/process-image/{strImage}", (string strImage, HttpContext http, CancellationToken token) =>
{
    http.Response.Headers.CacheControl = $"public,max-age={TimeSpan.FromHours(24).TotalSeconds}";
    return Results.Stream(stream => Resize(strImage, stream, token), "image/jpeg");
});

// Upload an image to blob storage from local wwwroot/img folder
// The following code requires an Azure storage account with the access key connection
// string stored in configuration.
app.MapPost("/up/{blobName}/{containerName}", (string blobName, string containerName) =>
{
    var conStr = builder.Configuration["blogConStr"];
    BlobContainerClient blobContainerClient = new BlobContainerClient(conStr, containerName);

    if (!blobContainerClient.Exists())
    {
        blobContainerClient.Create();
    }

    BlobClient blob = blobContainerClient.GetBlobClient(blobName);

    blob.Upload($"wwwroot/img/{blobName}");
});


app.Run();
