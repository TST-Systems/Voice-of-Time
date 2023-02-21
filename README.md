<a name="project-name-and-description"></a>
# Voice-of-Time
An open-source Voice- and Text-communication Software

<p align="center">
<img src="https://user-images.githubusercontent.com/72736935/216663183-103f73f5-9f90-4cf6-b658-bb332270406a.png">
</p>

## Table of Contents

- [Project Name and Description](#project-name-and-description)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
- [Contributions](#contributions)
- [License](#license)
- [Credits](#credits)
- [Acknowledgments](#acknowledgments)
- [FAQ](#faq)

<a name="getting-started"></a>
## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- Visual Studio 2022 or later

### Installation

1. Clone the repository to your local machine using `https://github.com/TST-Systems/Voice-of-Time.git`.
2. Open Visual Studio 2022.
3. Click on "File" > "Open" > "Project/Solution".
4. Navigate to the repository you just cloned and select the project file with the `.sln` extension.
5. Once the project is loaded, you should be able to build and run it by clicking on the "Start" button in the toolbar or by pressing `F5` on your keyboard.

*You can either select `Voice of Time` or `Voice of Time Server` as StartUp Project*

<a name="usage"></a>
## Usage
This Project contains three subprojekts: Voice of Time, Voice of Time Server and VoTCore
![Image](https://user-images.githubusercontent.com/72736935/216648066-bf4ea7b4-2918-408e-82ec-27c8529c0bb3.png)

### Core
Core libarys for Client and Server for features like:
- Communication between Client -> Server 
- Communication between Client -> Client
- De-/Enryption with RSA key
- Basic storage units
- Exeptions
- Futher features

### Client
The part for the enduser
- Contains all ClientData and partially information over the server and outher clients
- Manages connections with 'theoretically' infinit connections parralel
- Runs currently over cmd

### Server
A Serverinstace
- Contains all Public Information of its users
- Manages handling of messages for the server and outher users

### Test
Just a side projekt for testing the functionality of some features

<a name="contributions"></a>
## Contributions
We welcome contributions to this project. If you're interested in helping, take a look at the [open issues](https://github.com/your-username/your-project/issues) and see if there's anything you'd like to work on.

Here are some general guidelines for contributions:

1. Fork the repository and make a local clone of your fork.
2. Create a new branch for your changes and make your changes on that branch.
3. Commit your changes with a descriptive message.
4. Push your changes to your fork on GitHub.
5. Open a pull request to the original repository.

Note that by contributing to this project, you agree to release your changes under the same license as the original project.

<a name="license"></a>
## License
This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

<a name="credits"></a>
## Credits
- Timeplex(https://github.com/TLTimeplex) - Initial work and ongoing maintenance in the back-end
- Timkroe21(https://github.com/Timkroe21) - Initial work and ongoing maintenance in the front-end
- SalzstangeManga(https://github.com/SalzstangeManga) - Some contributions in the back-end

<a name="acknowledgments"></a>
## Acknowledgments
None ;(

<a name="faq"></a>
## FAQ
### Which OS will be supported
Currently only Windows. But we plan to provide at least a server version for linux.
