window.addEventListener("load", () => {
    const ui = window.ui;
    // Add a floating login button that calls /api/auth/login then preauthorizes
    const btn = document.createElement("button");
    btn.innerText = "Login & Authorize";
    btn.style.cssText = "position:fixed;top:10px;right:160px;z-index:9999;padding:6px 10px;";
    btn.onclick = async () => {
        const username = prompt("Username", "admin");
        const password = prompt("Password", "Admin#1234");
        if (!username || !password) return;

        const res = await fetch("/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password })
        });
        if (!res.ok) { alert("Login failed"); return; }
        const data = await res.json();
        const bearer = "Bearer " + data.accessToken;
        ui.preauthorizeApiKey("Bearer", bearer);
        localStorage.setItem("rbac.jwt", bearer);
        alert("Authorized!");
    };
    document.body.appendChild(btn);

    // Auto-apply stored token
    const saved = localStorage.getItem("rbac.jwt");
    if (saved) ui.preauthorizeApiKey("Bearer", saved);
});
