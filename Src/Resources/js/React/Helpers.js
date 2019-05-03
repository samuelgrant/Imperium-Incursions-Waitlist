export function DateFormat(string) {

    let date = new Date(string)
    let days = ["Sun", "Mon", "Tue", "Wed", "Thur", "Fri", "Sun"];
    let months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]

    return `${days[date.getDay()]} ${date.getDate()} ${months[date.getMonth()]}, ${date.getFullYear()}`;
}