var options = {
	chart: {
		width: 300,
		type: "pie",
	},
	labels: ["Team A", "Team B", "Team C", "Team D", "Team E"],
	series: [20, 20, 20, 20, 20],
	legend: {
		position: "bottom",
	},
	dataLabels: {
		enabled: false,
	},
	stroke: {
		width: 0,
	},
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
};
var chart = new ApexCharts(document.querySelector("#pie"), options);
chart.render();
