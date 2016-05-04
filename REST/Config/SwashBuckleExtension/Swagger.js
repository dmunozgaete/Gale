$(function () {
    var setToken_input = $("#explore");
    var LABEL_LOGIN = "Authenticate";
    var LABEL_LOGOUT = "Log Out";

    //Find the Security Definitions and add the according header's
    var token_input = $('#input_apiKey');
    token_input.attr("placeholder", "Put your access token here");

    setToken_input.one("click", function () {

        switch (setToken_input.html()) {
            case LABEL_LOGOUT:
                //--------------------------------
                // LOG OUT 
                if (localStorage) {
                    token_input.val("");
                    localStorage.clear();
                    //Remove all security definitions
                    for (var name in window.swaggerApi.securityDefinitions) {
                        window.swaggerUi.api.clientAuthorizations.remove(name);
                    }
                }
                tryToAuthenticate();
                //--------------------------------
                break;
            case LABEL_LOGIN:
                //--------------------------------
                // AUTHENTICATE 
                var token = token_input.val();
                if (token && token.trim() != "") {
                    for (var name in window.swaggerApi.securityDefinitions) {
                        var definition = window.swaggerApi.securityDefinitions[name];
                        window.swaggerUi.api.clientAuthorizations.add(name, new SwaggerClient.ApiKeyAuthorization(definition.name, 'Bearer ' + token, definition.in));

                        //window.authorizations.add(name, new ApiKeyAuthorization(definition.name, "Bearer " + token, definition.in));

                        //Try to save in local storage ;)
                        if (localStorage) {
                            localStorage.setItem('auth_' + name, token);
                        }
                    }
                }
                //--------------------------------
                break;
        }

        //remove default "api_key"
        setTimeout(function () {
            window.swaggerUi.api.clientAuthorizations.remove("api_key");
        }, 500);
    })

    var tryToAuthenticate = function () {
        //Try to get a security authorization
        var isAuthenticated = false;
        for (var name in window.swaggerApi.securityDefinitions) {
            var definition = window.swaggerApi.securityDefinitions[name];

            //Try to save in local storage ;)
            if (localStorage) {
                var token = localStorage.getItem('auth_' + name);
                if (token) {
                    window.swaggerUi.api.clientAuthorizations.add(name, new SwaggerClient.ApiKeyAuthorization(definition.name, 'Bearer ' + token, definition.in));

                    //window.authorizations.add(name, new ApiKeyAuthorization(definition.name, "Bearer " + token, definition.in));
                    token_input.val(token);
                    isAuthenticated = true;
                }
            }
        }
        if (isAuthenticated) {
            setToken_input.html(LABEL_LOGOUT);
        } else {
            setToken_input.html(LABEL_LOGIN);
        }
    }
    tryToAuthenticate();





    //Switch the label of the parameter to the "Comment", Wich in API is setted the Real Parameter Name
    var fixHeaderParams = function () {

        $(".operation-params").find("tr").each(function (idx, tr) {
            var tds = $(tr).children();

            var isHeaderParam = $(tds[3]).text() == "header";
            if (isHeaderParam) {

                //Re-position Label's HeaderName -> Comment
                var parameterName = $.trim($(tds[2]).text());

                //In server Mark the Parameter Name in Comment with *
                if (parameterName.indexOf("*") == 0) {

                    var headerName = $.trim($(tds[0]).text());

                    $(tds[2]).html("<strong>" + headerName + "</strong>");
                    $(tds[0]).html(parameterName.substring(1));
                }
            }
        });

    }

    fixHeaderParams();

    //Open the Gale Documentation Site when clicked in the logo!
    (function () {
        //IF LOCAL STORAGE??
        if (window['localStorage']) {
            var logo = $("#logo");
            var GALE_URL_SITE = localStorage.getItem("GALE_URL_SITE");
            logo.attr("href", GALE_URL_SITE);
            logo.attr("target", "_blank");
        }
    })();
});