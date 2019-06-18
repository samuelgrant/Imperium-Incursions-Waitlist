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

        return (
            <div>
                <div id="waitlistQueueHead">
                    <h5 className="pr-5 d-inline">Waitlist Queue</h5>
                    <h5 className="d-inline">
                        <i className="fas fa-info-circle" data-tip="Your Position only takes into account unique users<br/>and does not display their alts." data-multiline="true"></i>
                        {position}
                    </h5>
                    <ReactTooltip />
                </div>

                <div id="queues" className="py-4">
                    <div className="row text-center">
                        <div className="col-3">
                            DPS: 3
                        </div>

                        <div className="col-3">
                            LOGI: 3
                        </div>

                        <div className="col-3">
                            CAPITALS: 3
                        </div>
                    </div>
                </div>

                <div id="YourWaitTime">
                    <h5>Your Wait Time: {this.props.payload.yourWaitTime}</h5>
                    <button className="btn btn-danger float-right disabled">Leave the Waitlist <i className="fas fa-user-times"></i></button>
                </div>
            </div>
        )
    }
}