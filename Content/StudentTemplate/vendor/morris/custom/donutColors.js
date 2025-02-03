// Morris Donut
Morris.Donut({
	element: "donutColors",
	data: [
		{ value: 30, label: "foo" },
		{ value: 15, label: "bar" },
		{ value: 10, label: "baz" },
		{ value: 5, label: "A really really long label" },
	],
	backgroundColor: "#17181c",
	labelColor: "#17181c",
	colors: [
		"#2f477a",
		"#35508a",
		"#3b5999",
		"#4f6aa3",
		"#627aad",
		"#768bb8",
		"#899bc2",
		"#9daccc",
	],
	resize: true,
	hideHover: "auto",
	gridLineColor: "#dfd6ff",
	formatter: function (x) {
		return x + "%";
	},
});
