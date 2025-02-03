// Chart 1
var options1 = {
	chart: {
		height: 100,
		width: 250,
		type: "line",
		toolbar: {
			show: false,
		},
	},
	dataLabels: {
		enabled: false,
	},
	stroke: {
		curve: "smooth",
		width: 3,
	},
	series: [
		{
			name: "Views",
			data: [5, 10, 30, 15, 35, 25, 45],
		},
	],
	grid: {
		show: false,
	},
	xaxis: {
		categories: ["S", "M", "T", "W", "T", "F", "S"],
	},
	yaxis: {
		labels: {
			show: false,
		},
	},
	colors: ["#a7874b", "#9196a2", "#66a4ff"],
	markers: {
		show: false,
	},
};
var chart1 = new ApexCharts(document.querySelector("#views1"), options1);
chart1.render();

// Chart 2
var options2 = {
	chart: {
		height: 100,
		width: 250,
		type: "line",
		toolbar: {
			show: false,
		},
	},
	dataLabels: {
		enabled: false,
	},
	stroke: {
		curve: "smooth",
		width: 3,
	},
	series: [
		{
			name: "Views",
			data: [5, 10, 30, 15, 35, 25, 45],
		},
	],
	grid: {
		show: false,
	},
	xaxis: {
		categories: ["S", "M", "T", "W", "T", "F", "S"],
	},
	yaxis: {
		labels: {
			show: false,
		},
	},
	colors: ["#3b5999", "#9196a2", "#66a4ff"],
	markers: {
		show: false,
	},
};
var chart2 = new ApexCharts(document.querySelector("#views2"), options2);
chart2.render();
