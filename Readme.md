# Dotnet Todo.txt CLI

`dotnet-todo` is a .NET command line port of [Todo.txt](http://todotxt.org/) that tries to
remain faithful to the command line and functionality of the original shell script wherever
possible. As such, the [usage](#usage) below is a modified copy of the
[original on GitHub](https://github.com/todotxt/todo.txt-cli/blob/master/USAGE.md).

- [Installation](#installation)
- [Usage](#usage)
- [Actions](#actions)
- [Options](#options)
- [Configuration](#configuration)

## Installation

This program is a [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) and requires the latest version of
the [.NET SDK](https://dotnet.microsoft.com/download) to be installed. .NET 5.0 or newer is recommended.

### Testing Locally

Build it, then package it using the **Pack** command in Visual Studio or `dotnet pack`
on the command line. Until this package is published, install it using the following
command line from the solution root;

```sh
dotnet tool install -g --add-source .\src\todo\nupkg\ dotnet-todo
```

To update from a previous version,

```sh
dotnet tool update -g --add-source .\src\todo\nupkg\ dotnet-todo
```

### Installing from GitHub Packages

Whenever the version is updated in `src/todo/todo.csproj`, a merge to master will publish the NuGet package
to [GitHub Packages](https://github.com/rprouse?tab=packages). You can install or update from there.

First you must update your global NuGet configuration to add the package registry and include the GitHub Personal
Access Token (PAT). This file is in `%appdata%\NuGet\NuGet.Config` on Windows and in `~/.config/NuGet/NuGet.Config`
or `~/.nuget/NuGet/NuGet.Config` on Linux/Mac.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="Local" value="C:\temp" />
    <add key="Microsoft Visual Studio Offline Packages" value="C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\" />
    <add key="github" value="https://nuget.pkg.github.com/rprouse/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="rprouse" />
      <add key="ClearTextPassword" value="GITHUB_PAT" />
    </github>
  </packageSourceCredentials>
</configuration>
```

Once that is done, to install,

```sh
dotnet tool install -g dotnet-todo
```

And to update from a previous version,

```sh
dotnet tool update -g dotnet-todo
```

### Enabling Tab Completion

This program supports tab completion using `dotnet-suggest`. To enable, for each shell
you must install the `dotnet-suggest` global tool and adding a shim to your profile. This
only needs to be done once and work for all applications built using `System.CommandLine`.

Follow the [setup instructions](https://github.com/dotnet/command-line-api/blob/main/docs/dotnet-suggest.md)
for your shell.

## Usage

```shell
todo [-fhpantvV] [-d todo_config] action [task_number] [task_description]
```

### Warning

The Windows command line uses the `@` sign to indicate that command line arguments should be loaded from
the file after the @ sign. This is a problem when searching for a context in your task list which also uses
the command line. For example, if I wanted to search for my tasks with `@work`, I would normally try this and
get the following error;

```shell
> todo list @work
Response file not found 'work'
```

In Powershell, the above command does not filter on the context and returns all items. If you try to add
quotes, `todo list "@work"` in Powershell, then you get the same error.

I have not found a way to escape the `@` on the command line and adding quotes does not work. As a workaround
you can leave out the `@` sign and search using `todo list work`. This will also include tasks with the word
work, but that is minor. I may add `listcon` and `listpri` commands in the future.

If anyone has a proper workaround, please file an issue and I will update this.

Also note, that unlike the shell script version, quotes are required around any strings with spaces.

## Actions

### `add`
Adds "THING I NEED TO DO" to your todo.txt file on its own line.

Project and context notation optional.

```shell
todo add "THING I NEED TO DO +project @context"
todo a "THING I NEED TO DO +project @context"
```

### `addm`
Adds "FIRST THING I NEED TO DO" to your todo.txt on its own line and
adds "SECOND THING I NEED TO DO" to you todo.txt on its own line.

Project and context notation optional.

```shell
todo addm "FIRST THING I NEED TO DO +project1 @context" "SECOND THING I NEED TO DO +project2 @context"
```

### `addto`
Adds a line of text to any file located in the todo.txt directory.

For example, `addto inbox.txt "decide about vacation"`

```shell
todo addto DEST "TEXT TO ADD"
```

### `append`
Adds TEXT TO APPEND to the end of the task on line ITEM#.

```shell
todo append ITEM# "TEXT TO APPEND"
todo app ITEM# "TEXT TO APPEND"
```

### `archive`
Moves all done tasks from todo.txt to done.txt and removes blank lines.

```shell
todo archive
```

### `deduplicate`
Removes duplicate lines from todo.txt.

```shell
todo deduplicate
```

### `del`
Deletes the task on line ITEM# in todo.txt. If TERM specified, deletes only
TERM from the task.

```shell
todo del ITEM# [TERM]
todo rm ITEM# [TERM]
```

### `depri`
Deprioritizes (removes the priority) from the task(s) on line ITEM# in todo.txt.

```shell
todo depri ITEM#[, ITEM#, ITEM#, ...]
todo dp ITEM#[, ITEM#, ITEM#, ...]
```

### `do`
Marks task(s) on line ITEM# as done in todo.txt.

```shell
todo do ITEM#[, ITEM#, ITEM#, ...]
```

### `help`
Display help about usage, options, built-in and add-on actions, or just the usage
help for the passed ACTION(s).

```shell
todo help [ACTION...]
```

### `list`
Displays all tasks that contain TERM(s) sorted by priority with line numbers.  Each
task must match all TERM(s) (logical AND). Hides all tasks that contain TERM(s)
preceded by a minus sign (i.e. `-TERM`).

If no TERM specified, lists entire todo.txt.
​
```shell
todo list [TERM...]
todo ls [TERM...]
```

### `listall`
Displays all the lines in todo.txt AND done.txt that contain TERM(s) sorted by
priority with line  numbers. Hides all tasks that contain TERM(s) preceded by a
minus sign (i.e. `-TERM`).

If no TERM specified, lists entire todo.txt AND done.txt concatenated and sorted.

```shell
todo listall [TERM...]
todo lsa [TERM...]
```

### `listcon`
Lists all the task contexts that start with the @ sign in todo.txt.

If TERM specified, considers only tasks that contain TERM(s).

```shell
todo listcon [TERM...]
todo lsc [TERM...]
```

### `listfile`
Displays all the lines in SRC file located in the todo.txt directory, sorted by
priority with line numbers. If TERM specified, lists all lines that contain TERM(s)
in SRC file. Hides all tasks that contain TERM(s) preceded by a minus sign (i.e. `-TERM`).

Without any arguments, the names of all text files in the todo.txt directory are listed.

```shell
todo listfile [SRC [TERM...]]
todo lf [SRC [TERM...]]
```

### `listpri`
Displays all tasks prioritized PRIORITIES. PRIORITIES can be a single one (A) or a range
(A-C). If no PRIORITIES specified, lists all prioritized tasks. If TERM specified, lists
only prioritized tasks that contain TERM(s). Hides all tasks that contain TERM(s) preceded
by a minus sign (i.e. `-TERM`).

```shell
todo listpri [PRIORITIES] [TERM...]
todo lsp [PRIORITIES] [TERM...]
```

### `listproj`
Lists all the projects (terms that start with a `+` sign) in todo.txt. If TERM specified,
considers only tasks that contain TERM(s).

```shell
todo listproj [TERM...]
todo lsprj [TERM...]
```

### `move`
Moves a line from source text file (SRC) to destination text file (DEST). Both source
and destination file must be located in the directory defined in the configuration
directory. When SRC is not defined it's by default todo.txt.

```shell
todo move ITEM# DEST [SRC]
todo mv ITEM# DEST [SRC]
```

### `prepend`
Adds TEXT TO PREPEND to the beginning of the task on line ITEM#.

```shell
todo prepend ITEM# "TEXT TO PREPEND"
todo prep ITEM# "TEXT TO PREPEND"
```

### `pri`
Adds PRIORITY to task on line ITEM#.  If the task is already prioritized, replaces
current priority with new PRIORITY. PRIORITY must be a letter between A and Z.

```shell
todo pri ITEM# PRIORITY
todo p ITEM# PRIORITY
```

### `replace`
Replaces task on line ITEM# with UPDATED TODO.

```shell
todo replace ITEM# "UPDATED TODO"
```

## Options

### `-@`
Hide context names in list output.

### `-+`
Hide project names in list output.

### `-d CONFIG_FILE`
Use a configuration file other than the default `~/.todo/config`

### `-f`
Forces actions without confirmation or interactive input.

### `-p`
Plain mode turns off colors

### `-P`
Hide priority labels in list output.

### `-a`
Don't auto-archive tasks automatically on completion

### `-t`
Prepend the current date to a task automatically when it's added.

### `--version`
Displays version, license and credits

## Configuration

This program does **not** support the default `todo.cfg` file as provided
by the original shell script. Instead, this program uses a JSON file called
`~/.todo.json` in the user's home directory.

The defaults for this file put the todo files in a `Todo` directory in the
users `Documents` directory.

Allowed colors are `black, blue, cyan, gray, green, magenta, red, white,
yellow, darkBlue, darkCyan, darkGray, darkGreen, darkMagenta, darkRed,
darkYellow`. Either omit or set any color to `null` to use the default
terminal color.

You only need to add lines to `~/.todo.json` that you want to change.
Everything else will be set to the defaults listed below.

The format and defaults for this file are;

```json
{
  "todoDirectory": "C:\\Users\\username\\Documents\\Todo",
  "todoFile": "Todo.txt",
  "doneFile": "Done.txt",
  "reportFile": "Report.txt",
  // Colors for each priority from A to Z
  "priorities": {
    "A": {
      "color": "yellow",
      "backgroundColor": null
    },
    "B": {
      "color": "green",
      "backgroundColor": null
    },
    "C": {
      "color": "cyan",
      "backgroundColor": null
    }
  },
  // Color of done items
  "doneColor": {
    "color": "darkGray",
    "backgroundColor": null
  },
  // Color of any +projects within the text
  "projectColor": {
    "color": "red",
    "backgroundColor": null
  },
  // Color of any @contexts within the text
  "contextColor": {
    "color": "red",
    "backgroundColor": null
  },
  // Color of dates like 2020-10-22
  "dateColor": {
    "color": "magenta",
    "backgroundColor": null
  },
  // Color of the task numbers
  "numberColor": {
    "color": "gray",
    "backgroundColor": null
  },
  // The color of name value pairs like DUE:2020-10-22
  "metaColor": {
    "color": "darkCyan",
    "backgroundColor": null
  }
}
```

Here is very simple example, just changing the directory that your todo
files are stored in and using the defaults for everything else. Note
that the trailing slashes on the directory are optional.

```json
{
  "todoDirectory": "G:\\My Drive\\todo\\"
}
```
