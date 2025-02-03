document.addEventListener("DOMContentLoaded", function () {
	var calendarEl = document.getElementById("dayGrid");
	var calendar = new FullCalendar.Calendar(calendarEl, {
		headerToolbar: {
			left: "prevYear,prev,next,nextYear today",
			center: "title",
			right: "dayGridMonth,dayGridWeek,dayGridDay",
		},
		initialDate: "2023-10-10",
		navLinks: true, // can click day/week names to navigate views
		editable: true,
		dayMaxEvents: true, // allow "more" link when too many events
		events: [
			{
				title: "All Day Event",
				start: "2023-10-01",
				color: "#3b5999",
			},
			{
				title: "Long Event",
				start: "2023-10-07",
				end: "2023-10-10",
				color: "#2f477a",
			},
			{
				groupId: 999,
				title: "Birthday",
				start: "2023-10-09T16:00:00",
				color: "#4f6aa3",
			},
			{
				groupId: 999,
				title: "Birthday",
				start: "2023-10-16T16:00:00",
				color: "#35508a",
			},
			{
				title: "Conference",
				start: "2023-10-11",
				end: "2023-10-13",
				color: "#4f6aa3",
			},
			{
				title: "Meeting",
				start: "2023-10-14T10:30:00",
				end: "2023-10-14T12:30:00",
				color: "#627aad",
			},
			{
				title: "Lunch",
				start: "2023-10-16T12:00:00",
				color: "#768bb8",
			},
			{
				title: "Meeting",
				start: "2023-10-18T14:30:00",
				color: "#899bc2",
			},
			{
				title: "Interview",
				start: "2023-10-21T17:30:00",
				color: "#9daccc",
			},
			{
				title: "Meeting",
				start: "2023-10-22T20:00:00",
				color: "#b1bdd6",
			},
			{
				title: "Birthday",
				start: "2023-10-13T07:00:00",
				color: "#627aad",
			},
			{
				title: "Click for Google",
				url: "http://bootstrap.gallery/",
				start: "2023-10-28",
				color: "#4f6aa3",
			},
			{
				title: "Interview",
				start: "2023-10-20",
				color: "#3b5999",
			},
			{
				title: "Product Launch",
				start: "2023-10-29",
				color: "#35508a",
			},
			{
				title: "Leave",
				start: "2023-10-25",
				color: "#627aad",
			},
		],
	});

	calendar.render();
});
