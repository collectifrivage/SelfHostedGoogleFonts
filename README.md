# SelfHostedGoogleFonts

With this package you can easily self-host Google Fonts, which helps protect your users from Google's tracking.

## How to use

Simply add this code to your `Program.cs`:

```csharp
builder.Services.AddSelfHostedGoogleFonts()
    .AddFileSystemStorage();
```

This assumes that the files stored in the `wwwroot` folder are web-accessible. If that is not the case, you will need to configure the file system storage (see below) and setup some way (such as ASP.NET's `UseStaticFiles()`) to make a folder web-accessible.

With this config in place, you can then request a Google font. First, get the _spec_ of the font that you want. This can be gotten from the embed code in Google fonts. For example, if your embed code contains this:

```html
<link href="https://fonts.googleapis.com/css2?family=Roboto:ital,wght@0,400;0,700;0,900;1,400;1,700&display=swap" rel="stylesheet">
```

Then the spec is `Roboto:ital,wght@0,400;0,700;0,900;1,400;1,700`.

You can include this font in your markup in one of two ways:

### With a tag helper

```
@* This can be put at the top of your cshtml file, or in 
   the _ViewImports.cshtml file to make it available globally *@
@addTagHelper *, SelfHostedGoogleFonts
```
```html
@* Place this where you want the <link> tag (for example, inside your <head>) *@
<google-fonts spec="Roboto:ital,wght@0,400;0,700;0,900;1,400;1,700" />
```

If you need multiple fonts, use this syntax instead:

```html
<google-fonts>
    <google-font spec="spec1"/>
    <google-font spec="spec2"/>
    <google-font spec="..."/>
</google-fonts>
```

### With an HtmlHelper extension

```csharp
@await Html.AddGoogleFontsAsync("Roboto:ital,wght@0,400;0,700;0,900;1,400;1,700")
```

You can also specify multiple specs: `AddGoogleFontsAsync("spec1", "spec2", ...)`

## Customizing the file system storage

```csharp
.AddFileSystemStorage(options => {
    options.RootPath = "";
    options.PublicPathPrefix = "google-fonts/";
});
```

- `RootPath`: the folder where fonts and stylesheets will be saved. By default, this will be `{IWebHostEnvironment.WebRootPath}/{PublicPathPrefix}`, which usually maps to `wwwroot/google-fonts/`.
- `PublicPathPrefix`: a URL segment added in front of public URLs. Defaults to "google-fonts".

## How it works

It builds the Google Fonts stylesheet URL from the specs you specify. If this stylesheet was already processed, a `<link>` tag is created using the self-hosted URL and the work is done.

Otherwise, the stylesheet is downloaded, and is scanned to find all font files that it references. Those fonts are downloaded and saved to the storage system, and then the stylesheet is updated to use the self-hosted font URLs. Finally the stylesheet is saved to the storage system, before creating the `<link>` tag as usual. 