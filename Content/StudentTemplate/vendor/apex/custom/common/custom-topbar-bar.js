// Chart 1
var options1 = {
	series: [
		{
			data: [15, 25, 35, 45, 55, 65, 75, 85, 95, 65, 45],
		},
	],
	chart: {
		type: "bar",
		width: 80,
		height: 40,
		sparkline: {
			enabled: true,
		},
	},
	plotOptions: {
		bar: {
			columnWidth: "80%",
		},
	},
	labels: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11],
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	colors: ["#3b5999", "#9196a2", "#66a4ff"],
	tooltip: {
		fixed: {
			enabled: false,
		},
		x: {
			show: false,
		},
		y: {
			title: {
				formatter: function (seriesName) {
					return "";
				},
			},
		},
		marker: {
			show: false,
		},
	},
};
var chart1 = new ApexCharts(document.querySelector("#coffee"), options1);
chart1.render();

// Chart 2
var options2 = {
	series: [
		{
			data: [15, 25, 35, 45, 55, 65, 75, 85, 95, 65, 45],
		},
	],
	chart: {
		type: "bar",
		width: 80,
		height: 40,
		sparkline: {
			enabled: true,
		},
	},
	plotOptions: {
		bar: {
			columnWidth: "80%",
		},
	},
	labels: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11],
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	colors: ["#a7874b", "#3b5999", "#66a4ff"],
	tooltip: {
		fixed: {
			enabled: false,
		},
		x: {
			show: false,
		},
		y: {
			title: {
				formatter: function (seriesName) {
					return "";
				},
			},
		},
		marker: {
			show: false,
		},
	},
};
var chart2 = new ApexCharts(document.querySelector("#cakes"), options2);
chart2.render();

// Chart 3
var options3 = {
	series: [
		{
			data: [15, 25, 35, 45, 55, 65, 75, 85, 95, 65, 45],
		},
	],
	chart: {
		type: "bar",
		width: 80,
		height: 40,
		sparkline: {
			enabled: true,
		},
	},
	plotOptions: {
		bar: {
			columnWidth: "80%",
		},
	},
	labels: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11],
	xaxis: {
		crosshairs: {
			width: 1,
		},
	},
	colors: ["#1fa278", "#9196a2", "#66a4ff"],
	tooltip: {
		fixed: {
			enabled: false,
		},
		x: {
			show: false,
		},
		y: {
			title: {
				formatter: function (seriesName) {
					return "";
				},
			},
		},
		marker: {
			show: false,
		},
	},
};
var chart3 = new ApexCharts(document.querySelector("#burgers"), options3);
chart3.render();
