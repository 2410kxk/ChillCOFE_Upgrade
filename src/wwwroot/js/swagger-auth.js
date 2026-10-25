window.chillSwaggerRequest = function (request) {
    var target = new URL(request.url, window.location.origin);

    if (target.origin !== window.location.origin) {
        return request;
    }

    request.credentials = "same-origin";

    var method = (request.method || "GET").toUpperCase();

    if (["POST", "PUT", "PATCH", "DELETE"].indexOf(method) === -1) {
        return request;
    }

    return fetch("/dev/antiforgery", {
        credentials: "same-origin",
        cache: "no-store"
    })
    .then(function (response) {
        if (!response.ok || response.redirected) {
            throw new Error(
                "Hãy đăng nhập Admin hoặc Staff trên website trước."
            );
        }

        return response.json();
    })
    .then(function (data) {
        if (!data.requestToken) {
            throw new Error("Không lấy được token bảo vệ.");
        }

        request.headers = request.headers || {};
        request.headers["RequestVerificationToken"] = data.requestToken;

        return request;
    });
};