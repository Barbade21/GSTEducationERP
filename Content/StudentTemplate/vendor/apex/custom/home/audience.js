var options = {
	chart: {
		height: 330,
		type: "area",
		toolbar: {
			show: false,
		},
	},
	dataLabels: {
		enabled: false,
	},
	stroke: {
		curve: "smooth",
		width: 2,
	},
	plotOptions: {
		bar: {
			columnWidth: "30%",
		},
	},
	series: [
		{
			name: "Audience",
			type: "line",
			data: [10, 40, 15, 30, 20, 35, 20, 10, 31, 43, 56, 29],
		},
		{
			name: "Revenue",
			type: "area",
			data: [2, 8, 35, 7, 50, 30, 51, 35, 42, 20, 33, 67],
		},
	],
	grid: {
		borderColor: "#dfd6ff",
		strokeDashArray: 5,
		xaxis: {
			lines: {
				show: true,
			},
		},
		yaxis: {
			lines: {
				show: false,
			},
		},
		padding: {
			top: 0,
			right: 0,
			bottom: 0,
			left: 0,
		},
	},
	xaxis: {
		categories: [
			"Jan",
			"Feb",
			"Mar",
			"Apr",
			"May",
			"Jun",
			"Jul",
			"Aug",
			"Sep",
			"Oct",
			"Nov",
			"Dec",
		],
	},
	yaxis: {
		labels: {
			show: false,
		},
	},
	colors: ["#c5c9d1", "#3b5999"],
	markers: {
		size: 0,
		opacity: 0.3,
		colors: ["#c5c9d1", "#3b5999"],
		strokeColor: "#ffffff",
		strokeWidth: 2,
		hover: {
			size: 7,
		},
	},
	tooltip: {
		y: {
			formatter: function (val) {
				return val + "k";
			},
		},
	},
};

var chart = new ApexCharts(document.querySelector("#audience"), options);

chart.render();
