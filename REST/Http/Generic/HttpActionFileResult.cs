using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Http
{
    public abstract class HttpActionFileResult : Gale.REST.Http.HttpBaseActionResult
        {
            HttpRequestMessage _request;    //Only for Content Negotiation

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="request"></param>
            public HttpActionFileResult(HttpRequestMessage request)
            {
                _request = request;
            }

            /// <summary>
            /// Save File
            /// </summary>
            /// <param name="content"></param>
            public abstract HttpResponseMessage SaveFiles(List<HttpContent> files);

            /// <summary>
            /// Implementacion de proceso custom para la insercion de datos, utilizando el SP "PA_MAE_UNS_USUARIO" para insertar un usuario.
            /// </summary>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public override Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
            {
                //------------------------------------------------------------------------------------------------------------------------
                //GUARD EXCEPTION   
                Gale.Exception.RestException.Guard(() => !_request.Content.IsMimeMultipartContent(), System.Net.HttpStatusCode.UnsupportedMediaType, "ONLY_MULTIPART_SUPPORTED", "");
                //------------------------------------------------------------------------------------------------------------------------

                var provider = new MultipartMemoryStreamProvider();

                var task = _request.Content.ReadAsMultipartAsync(provider).
                    ContinueWith<HttpResponseMessage>(o =>
                    {
                        //------------------------------------------------------------------------------------------------------------------------
                        //GUARD EXCEPTION
                        Gale.Exception.RestException.Guard(() => o.IsFaulted, "FILE_MAXLENGTH_ERROR","");
                        //------------------------------------------------------------------------------------------------------------------------

                        return SaveFiles(provider.Contents.ToList());
                    }
                );
                return task;
            }

        }

    }
