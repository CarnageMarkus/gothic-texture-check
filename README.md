# Gothic Texture Check CLI

Simple command line tool which checks folders for `.tga` or `.TEX` files and report if there are any missing animated or varied textures. 

### Description

Gothic game textures require specific naming in order to work properly.

There are three types:

* Animated
* Varied (Visual skin)
* Varied and Colored

The biggest concern here are the textures describing variations and color, such as `HUM_HEAD_V0_C0`.
In order to use texture `HUM_HEAD_V2_C1`, there must exist all the textures of lower order **(all variations up to number 2 each having color variant up to number 1)**.


| C\V | V0             | V1             | V2             |
|-----|----------------|----------------|----------------|
| C0  | HUM_HEAD_V0_C0 | HUM_HEAD_V1_C0 | HUM_HEAD_V2_C0 |
| C1  | HUM_HEAD_V0_C1 | HUM_HEAD_V1_C1 | HUM_HEAD_V2_C1 |


### Features

* Find duplicates
* Discover missing texture variants
* Discover missing animated texture frames

### Installation

Launch installer, app will be installed into `%appdata%`. Make sure you have it in your `PATH`

### Usage
Open `cmd` or `powershell` and use `gtexcheck` with appropriate parameters.

```text
Usage: gtexcheck [options]

Options:

  --help                 output usage information
  -v, --verbose          set app to verbose mode.
  -i, --interactive      the app will behave as with no parameters -> interactively asking what to do next.
  -d, --directory <path> path to the directory containing textures.
                         Usualy path to 'Gothic/_Work/Data/Textures'
                         or 'Gothic/_Work/Data/Textures/compiled/'
  -r, --recursive        recursively search all subfolders when looking for textures.
  --TGA                  include .tga extension when doing texture check.
  --TEX                  include .tex extension when doing texture check.
  -t, --tableStyle       change table style shown in verbose mode (Default, MarkDown, Alternative, Minimal).
```
#### Examples:


##### from Textures directory
```
  $ gtexcheck                                               <= quick check.

  $ gtexcheck -v -i                                         <= (Recomended use) verbose and interactive
```

##### from anywhere...
```
  $ gtexcheck -d <gothic_root/_Work/Data/Textures> -r --TEX <= Quick check for missing or duplicated .TEX files.
                                                               The tool just prints all info without stopping

  $ gtexcheck -d <gothic_root/_Work/Data/Textures> -r --TGA <= Quick check for missing or duplicated .TGA files.

  $ gtexcheck -d <gothic_root/_Work/Data/Textures> -i       <= Run interactive check of textures

  $ gtexcheck -d <gothic_root/_Work/Data/Textures> -v -i    <= Run interactive check with verbose output (more info)
```


#### Screenshots

Table shows missing textures by `--`.
`RED` indicates which textures **won't work** because of them.

![Alt text](img/screenshot_0.png?raw=true "Main interface")
![Alt text](img/screenshot_1.png?raw=true "Main interface")
![Alt text](img/screenshot_2.png?raw=true "Main interface")
![Alt text](img/screenshot_3.png?raw=true "Main interface")
