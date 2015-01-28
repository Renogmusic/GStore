How to Add Themes:

Most of these themes are plain as downloaded from bootswatch.com
All you need to do to add a bootswatch theme is download the bootstrap.min.css into a new folder in ~/content/Server/Themes, then go to the database to add the theme name and folder to a client.

Optionally, you can use .LESS files to customize the themes.  Here is an example of how to make theme files from bootswatch.less files


step 1 - prepare ~/Content/bootstrap/mixins.less
	Add the following to top of ~/content/bootstrap/mixins.less (it may already be there)
@charset "utf-8";

	save mixins.less and make sure web essentials updates the .less file and creates a mixins.css

Step 2 - Prepare ~/Content/Server/Themes/{ThemeName}/variables.less   
	Add the following to the top of Variables.less
@charset "utf-8";

	save file, Web Essentials will compile the .LESS file into a .css file, a .css.map file, and a .min.css file

Step 3 - Add the following to bootswatch.less
@charset "utf-8";
@import "../../../bootstrap/mixins";
@import "variables";

save file. Web Essentials should compile out a bootswatch.css file and bootswatch.min.css

Step 4 - Compile Bootstrap.min.css
	Copy Bootstrap.less file from another theme, i.e.
	~\Content\Server\Themes\Original\bootstrap.less


for reference: the first 3 lines should be   
// Core variables and mixins
@import "variables.less";
@import "../../../bootstrap/mixins.less";

the last line of the file should be

//Bootswatch Theme Include
@import "bootswatch";

add a line to the file, then remove the line, and save the file (triggers web essentials to re-compile it)

Save Bootstrap.less  and Web Essentials will compile out a Bootstrap.min.css file.  This is the file you will use for the theme.

Now log into system admin section of the site, add theme, and select the new folder.
You will see an OK next to the folder name if your Bootstrap.min.css file was found.

