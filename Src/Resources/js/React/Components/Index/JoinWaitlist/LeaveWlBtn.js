import React, { Component } from 'react';

export default class LeaveWaitlistBtn extends Component {
    leaveWaitlist() {
        $.ajax({
            type: 'delete',
            uri: this.props.baseUri,
            data: { pilot_id: "" }
        }).done((data) => {
            this.props.u();
        }).fail((err) => {
            console.error(`[React/LeaveWlBtn@leaveWaitlist] Error leaving the waitlist: ${err.responseText}`)
        })
    }

    render() {
        return (
            <button className="btn btn-danger float-right" onClick={this.leaveWaitlist.bind(this)} disabled={this.props.isOnWl}>
                Leave the Waitlist <i className="fas fa-user-times"></i>
            </button>
        );
    }
}