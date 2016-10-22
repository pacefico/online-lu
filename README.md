# CLOUD-LU: PROGRAMAÇÃO PARALELA PARA GPU UTILIZANDO PLATAFORMAS DE DESENVOLVIMENTO EM SERVIÇOS DE NUVEM (AZURE)
Paulo Figueiredo - Faculdade de Tecnologia - UNICAMP @2014

Trabalho de conclusão de curso submetido à Universidade Estadual de Campinas, como parte dos requisitos obrigatórios para obtenção do grau de Tecnólogo em Análise e Desenvolvimento de Sistemas
O texto, os detalhes da implementação, do projeto e todas as referências utilizadas estão no [pdf aqui][tcc_pdf]



## Resumo
Com o passar dos anos o poder de processamento dos computadores cresceu. No entanto, mesmo com a contínua produção de novos chips com maior poder de processamento, novas abordagens que utilizam o conceito do paralelismo têm sido propostas para reduzir o tempo de processamento. Uma dessas propostas é a junção do conceito do paralelismo ao sistema de nuvens computacionais,visando permitir execução paralela com característica altamente distribuída. Nesse sentido, neste trabalho, desenvolvemos um sistema CloudLU, capaz de integrar os conceitos de computação na nuvem com o de distribuição de dados para processamento paralelo em um ou vários nós computacionais que podem estar dispersos geograficamente. Para demonstração de conceitos, o sistema CloudLU foi desenvolvido baseado na plataforma de desenvolvimento Microsoft .NET, através de uma aplicação para decomposição LU de matrizes em unidades de processamento gráfico (GPU) interligada a uma aplicação também baseada na mesma plataforma, para os serviços de nuvem Windows Azure. Com este sistema, se verificou que a distribuição facilitada pela nuvem e o processamento via GPU permitiu ganhos de até 7 vezes em relação ao processamento sequencial local.
Palavras-chave: Paralelismo, Computação Distribuída, Nuvem, Plataforma de desenvolvimento, Azure, Decomposição Matricial LU, Gpu, Matemática.

## Abstract
Over the years the processing power of computers has grown. However, even with the continuous production of new chips with more processing power, new approaches that use the concept of parallelism have been proposed to reduce the processing time. One such approach is the junction the concept of parallelism to the cloud computing system, in order to enable parallel execution with highly distributed feature. Accordingly, in this work, we developed the CloudLU system capable to incorporate the concepts of cloud computing with the distribution of data for parallel processing on one or more computational nodes that can be geographically dispersed. For demonstration of concepts, the CloudLU system was developed based on the Microsoft development framework. NET, through an application for LU decomposition of matrices on graphics processing units (GPU) also linked to an application based on the same framework for the Windows Azure cloud services. With this systemit has been found that the distribution of the cloud facilitated and enabled GPU processing gain of up to 7 times compared to local sequential processing.
Key words: Parallel, Distributed Computing, Cloud, Development frameworks, Azure, Matrix LU Decomposition, Gpu, Math.


## O Sistema

O objetivo geral do sistema é disponibilizar uma interface para o usuário, o qual poderá submeter arquivos de dados contendo uma matriz a ser decomposta em triangular superior e inferior. Esse sistema, por meio da nuvem, distribuirá esses arquivos entre os nós de processamento. Os nós de processamento utilizarão um aplicativo de processamento para a decomposição LU utilizando GPU com o modelo de programação CUDA. Uma vez finalizado o processamento nos nós, o sistema retornará os resultados ao sistema na nuvem.

## Arquitetura

![Arquitetura do Sistema][screen_architecture]

A Figura apresenta o fluxo demarcado na cor laranja representando o fluxo de solicitação de processamento, enquanto que o fluxo demarcado na cor verde, representa o fluxo de resultado deste processamento. para então detalhá-los a seguir. 

O usuário acessa a função web (1), responsável pelo gerenciamento do sistema, abre a página de nova execução, insere os parâmetros da execução, em seguida o sistema envia a informação para o banco de dados (2), armazenando os parâmetros da execução solicitada, a função web envia os dados ao recipiente de itens a processar, que armazenará os arquivos necessários a execução da decomposição LU, finalizado a cópia dos arquivos (3) a função web envia uma mensagem para a fila de mensagens a processar (4), contendo os parâmetros da execução. Um dos nós de processamento monitora a fila de mensagens a processar (5), e detectando a presença de alguma mensagem, visualiza seu conteúdo e inicia o processo de cópia dos arquivos do recipiente elencado em (3), para si (6), compõe-se novamente então, a matriz de elementos a serem processados na memória principal deste nó, executa-se posteriormente a cópia desta matriz da memória principal para a memória da GPU e dispara-se o processo de decomposição LU (7). Ao finalizar o processo de decomposição LU, o sistema gera então novos arquivos de resultado e os armazena no recipiente do serviços de dados de itens processados (8), ao final deste processo, encaminha uma mensagem com os dados e resumo dos tempos desta execução para a fila de itens processados (9). A função de trabalho por sua vez, monitorando a fila de itens processados, verifica a presença da mensagem,visualiza e processa seu conteúdo (10), enviando seus resultados para o banco de dados (11), onde por fim ficará disponível para que o usuário tenha acesso ao histórico e ao arquivo de resultado final da decomposição LU (12).


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