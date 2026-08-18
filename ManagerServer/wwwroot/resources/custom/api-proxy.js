window.addEventListener("message", async (event) => {
    const { data } = event;
    if (data?.type !== "api-request") return;

    const { requestId, method, path, params, body } = data;

    try {
        const { status, body: resultBody } = await handleApiRequest(method, path, params, body);

        event.source.postMessage({
            type: "api-response",
            requestId,
            status,
            body: resultBody,
            error: null
        }, event.origin);
    } catch (err) {
        console.log(err);
        event.source.postMessage({
            type: "api-response",
            requestId,
            status: 500,
            body: null,
            error: err.message
        }, event.origin);
    }
});

async function handleApiRequest(method, path, params = {}, body = null) {
    const url = new URL(path, location.origin);

    Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
            url.searchParams.append(key, value);
        }
    });

    const headers = { "Content-Type": "application/json" };
    const business = typeof MANAGER_BUSINESS !== "undefined" ? MANAGER_BUSINESS : null;
    if (business != null) {
        headers["Manager-Business"] = encodeURIComponent(`${business}`);
    }

    const response = await fetch(url.toString(), {
        method,
        headers,
        body: method !== "GET" && body != null ? JSON.stringify(body) : null
    });

    const contentType = response.headers.get("Content-Type") || "";
    const responseBody = contentType.includes("application/json")
        ? await response.json()
        : await response.text();

    return {
        status: response.status,
        body: responseBody
    };
}
