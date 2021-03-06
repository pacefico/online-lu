# CLOUD-LU: Parallel code for gpu system using .Net framework based on Azure Cloud services
Paulo Figueiredo - Faculty of Technology - State University of Campinas - UNICAMP @2014

Project and dissertation thesis submitted to the State University of Campinas, as part of the mandatory requirements for the degree of Technologist in Systems Analysis and Development

The thesis, details, code implementation, e all references can be found [here][tcc_pdf] only in Portuguese.

For any question you may have, please contact me at pacefico@gmail.com


## Abstract

Over the years the processing power of computers has grown. However, even with the continuous production of new chips with more processing power, new approaches that use the concept of parallelism have been proposed to reduce the processing time. 

One such approach is the junction the concept of parallelism to the cloud computing system, in order to enable parallel execution with highly distributed feature. 

Accordingly, in this work, we developed the CloudLU system capable to incorporate the concepts of cloud computing with the distribution of data for parallel processing on one or more computational nodes that can be geographically dispersed.

For demonstration of concepts, the CloudLU system was developed based on the Microsoft development framework. NET, through an application for LU decomposition of matrices on graphics processing units (GPU) also linked to an application based on the same framework for the Windows Azure cloud services. 

With this system it has been found that the distribution of the cloud facilitated and enabled GPU processing gain of up to 7 times compared to local sequential processing.


Key words: Parallel, Distributed Computing, Cloud, Development frameworks, Azure, Matrix LU Decomposition, Gpu, Math.


## System description


The overall objective of the system is to provide an interface for the user, which may submit data files containing a matrix to be decomposed into upper and lower triangular. 

This system, through the Azure cloud, distribute these files between processing nodes. 

The process will then use a processing application to the LU decomposition with GPU with CUDA programming model. 

Once finished processing the nodes, the system returns the results to the system in the cloud.


## Architecture

![Arquitetura do Sistema][screen_architecture]


The Figure shows the flow marked in orange represents a processing request flow, while the flow marked in green represents results of this processing, and then there are details below.

The user accesses web function (1), responsible for system management.

The user opens a new run page, input run parameters, then the system sends the information to the database (2), storing the parameters of the requested execution.

The web function sends the data to the container processing items, which will store the files necessary to run the LU decomposition.

Ending file copy (3) the web function sends a message to the message queue "to process" (4), with the execution parameters.

One of the processing nodes monitors the message queue "to process" (5), and once detected the presence of a message, the node analyze its contents and begins the process by copying the container file part listed in (3) for (6)

It recompose then the array elements to be processed in main memory of this node, subsequently copy a matrix from main memory to GPU memory and starts the LU decomposition process (7).

At the end of the LU decomposition process, the system generates new result files and stores them in the container of processed items data services (8).

Once finished, the system sends a message to the data and summarize the time of this execution and put it into the queue of processed items (9).

The work function in turn, monitoring the queue of processed items, checks the presence of a message and processes its content (10), sending after, the results to the database (11).

And then, these results will be available for the user, who can access the history and the final result of the LU decomposition file (12).

## Technologies, language and tools involved

- Nvidia Gpu 
- Nvidia CUDA Model
- Microsfot Azure Services
- Microsoft Visual Studio
- SQL Database
- .Net Framework
- Asp.Net
- C# Programming Language
- C Language
- Rest, Json, Entity Framework, Linq


## Copyright

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

[screen_architecture]: /Project/arquitetura.png "Arquitetura do Sistema"
[tcc_pdf]: https://raw.githubusercontent.com/pacefico/online-lu/master/Project/tcc-paulo-final.pdf
