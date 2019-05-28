# Escc.Umbraco.SiteMap

Publish a search engine sitemap of all media items in the Umbraco media library.

The sitemap can be accessed at `https://hostname/sitemap/mediafiles/`. 

The media URLs are relative, and are appended to the value of a `cdnDomain` app setting in `web.config`, so to achieve a URL of `https://www.example.org/media/1234/example.jpg` you need the following in `web.config`:

	<configuration>
		<appSettings>
			<add key="cdnDomain" value="https://www.example.org" />
		</appSettings>
	</configuration>