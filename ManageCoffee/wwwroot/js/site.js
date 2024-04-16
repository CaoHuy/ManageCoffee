let sidebarItems = document.querySelectorAll(".sidebar-item.has-sub");
for (var i = 0; i < sidebarItems.length; i++) {
  let sidebarItem = sidebarItems[i];
  sidebarItems[i]
    .querySelector(".sidebar-link")
    .addEventListener("click", function (e) {
      e.preventDefault();

      let submenu = sidebarItem.querySelector(".submenu");
      submenu.classList.toggle("active");
      slideToggle(submenu, 300);
    });
}

document.querySelector(".burger-btn").addEventListener("click", () => {
  document.getElementById("sidebar").classList.toggle("active");
});

document.querySelector(".sidebar-hide").addEventListener("click", () => {
  document.getElementById("sidebar").classList.toggle("active");
});

// Perfect Scrollbar Init
if (typeof PerfectScrollbar == "function") {
  const container = document.querySelector(".sidebar-wrapper");
  const ps = new PerfectScrollbar(container, {
    wheelPropagation: false,
  });
}
