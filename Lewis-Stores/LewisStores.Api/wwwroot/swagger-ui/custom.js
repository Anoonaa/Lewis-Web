window.addEventListener("load", () => {
  const topbarLink = document.querySelector(".swagger-ui .topbar-wrapper a");
  if (topbarLink) {
    topbarLink.setAttribute("target", "_self");
  }

  const infoSection = document.querySelector(".swagger-ui .info");
  if (!infoSection) {
    return;
  }

  const notice = document.createElement("div");
  notice.style.marginTop = "12px";
  notice.style.padding = "12px";
  notice.style.border = "1px solid #c9dcdf";
  notice.style.borderRadius = "8px";
  notice.style.background = "#f0faf8";
  notice.style.color = "#11424c";
  notice.style.fontSize = "13px";
  notice.innerHTML = "<strong>Quick Start:</strong> Use <em>POST /api/Auth/login</em> to get a token, then click <em>Authorize</em> and paste the token value.";

  infoSection.appendChild(notice);
});
