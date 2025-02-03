var options = {
	series: [
		{
			name: "Tickets",
			data: [1100, 880, 740, 548, 330, 200],
		},
	],
	chart: {
		type: "bar",
		height: 300,
		toolbar: {
			show: false,
		},
	},
	dataLabels: {
		enabled: false,
	},
	plotOptions: {
		bar: {
			borderRadius: 0,
			horizontal: true,
			distributed: true,
			barHeight: "80%",
			isFunnel: true,
		},
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
	dataLabels: {
		enabled: true,
		formatter: function (val, opt) {
			return opt.w.globals.labels[opt.dataPointIndex];
		},
		dropShadow: {
			enabled: true,
		},
	},
	xaxis: {
		categories: ["Closed", "Hold", "Resolved", "Waiting", "On Going", "Total"],
	},
	legend: {
		show: true,
	},
};

var chart = new ApexCharts(document.querySelector("#funnel"), options);
chart.render();
