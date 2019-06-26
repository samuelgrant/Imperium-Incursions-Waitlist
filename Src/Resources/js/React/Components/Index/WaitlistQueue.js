import React, { Component } from 'react';
import ReactTooltip from 'react-tooltip'

export default class WaitlistQueue extends Component {
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
                    <div className="col-3">
                        {`${queue.Name}: ${queue.Count}`}
                    </div>
                );
            });
        }

        return (
            <div>
                <div id="waitlistQueueHead" className="row">
                    <div className="col-12 pb-4">
                        <h5 className="pr-5 d-inline">Waitlist Queue</h5>
                    </div>

                    <div className="col-lg-6 col-md-12">
                        <i className="fas fa-info-circle" data-tip="Your Position only takes into account unique users<br/>and does not display their alts." data-multiline="true"></i>
                        {position}
                        <ReactTooltip />
                    </div>

                    <div className="col-lg-6 col-md-12">
                        <i className="fas fa-hourglass-half"></i> Your Wait Time: {this.props.payload.yourWaitTime}
                    </div>

                    <div className="col-12 py-5">
                        <div className="row text-center">
                            {queues}
                        </div>
                    </div>
                </div>
            </div>
        )
    }
}