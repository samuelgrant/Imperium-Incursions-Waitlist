export function DateFormat(string) {

    let date = new Date(string)
    let days = ["Sun", "Mon", "Tue", "Wed", "Thur", "Fri", "Sun"];
    let months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]

    return `${days[date.getDay()]} ${date.getDate()} ${months[date.getMonth()]}, ${date.getFullYear()}`;
}

// Takes in an auth name and an array of pilots, returns the pilot that matches the auth name || the first pilot
export function AccountPilot(auth_name, pilotsArray) {

    // Pilots Array was not an array, pass a default object back
    if (!pilotsArray instanceof Array)
        return { id: 0, name: "", corporation: { id: 0, name: "" }, alliance: { id: 0, name: ""}}

    for (let i = 0; i < pilotsArray.length; i++) {
        if (pilotsArray[i].name.toLowerCase() == auth_name.toLowerCase())
            return pilotsArray[i];
    }

    // No match found
    return pilotsArray[0];
}