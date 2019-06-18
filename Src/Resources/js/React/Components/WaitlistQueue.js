import React, { Component } from 'react';
import ReactTooltip from 'react-tooltip'

export default class WaitlistQueue extends Component {

    leaveWaitlist() {
        $.ajax({
            type: 'delete',
            uri: this.props.baseUri,
            data: { pilot_id: ""}
        }).done((data) => {
            this.props.u();
        }).fail((err) => {
            console.error(`[React/WaitlistQueue@leaveWaitlist] Error leaving the waitlist: ${err.responseText}`)
        })
    }

    render() {
        let position;
        if (this.props.payload && this.props.payload.yourPos) {
            position = `Your Position: ${this.props.payload.yourPos || ""} / ${this.props.payload.totalWaiting || 0}`;
        } else {
            position = `People waiting: ${this.props.payload.totalWaiting || 0}`;
        }

        let queues;
        if (this.props.payload.queues) {

            queues = Object.keys(this.props.payload.queues).map((key) => {
                return { "Name": key, "Count": this.props.payload.queues[key]};
            });

            queues = queues.map((queue) => {
                return (
                    <div className="col-3 text">
                        {`${queue.Name}: ${queue.Count}`}
                    </div>
                );
            });
        }

        return (
            <div>
                <div id="waitlistQueueHead" className="pb-4">
                    <h5 className="pr-5 d-inline">Waitlist Queue</h5>
                    <h5 className="d-inline">
                        <i className="fas fa-info-circle" data-tip="Your Position only takes into account unique users<br/>and does not display their alts." data-multiline="true"></i>
                        {position}
                    </h5>
                    <ReactTooltip />
                </div>

                <div id="queues" className="pb-4">
                    <div className="row text-center">
                        {queues}
                    </div>
                </div>

                <div id="YourWaitTime">
                    <h5>Your Wait Time: {this.props.payload.yourWaitTime}</h5>
                    <button className="btn btn-danger float-right" onClick={this.leaveWaitlist.bind(this)}>Leave the Waitlist <i className="fas fa-user-times"></i></button>
                </div>
            </div>
        )
    }
}