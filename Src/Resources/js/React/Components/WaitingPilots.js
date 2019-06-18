import React, { Component } from 'react';
import Card from './Card';
import { Pilot } from './EsiUi';

export default class WaitingPilot extends Component {

    removePilot(pilot_id) {
        $.ajax({
            type: 'delete',
            url: `${this.props.baseUri}`,
            data: {
                pilot_id: pilot_id
            }
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`React/Components/WaitingPilots {WaitingPilot@removePilot} - Error leaving the waitlist`, err.responseText);
        })  
    }

    render() {
        let pilots;
        if (this.props.pilots) {
            pilots = this.props.pilots.map((waiting) => {
                return (
                    <tr>
                        <td><img width="32px" src={`https://image.eveonline.com/Character/${waiting.id}_32.jpg`} /></td>
                        <td><Pilot pilot={waiting} /></td>
                        <td><buton className="btn btn-danger" onClick={this.removePilot.bind(this, waiting.id)}>Remove <i className="fas fa-user-times"></i></buton></td>
                    </tr>
                )
            });
        }

        let content;
        if (this.props.pilots && this.props.pilots.length > 0) {
            content = (
                <div className="table-responsive">
                    <table className="table">
                        <thead>
                            <tr>
                                <th></th>
                                <th>Pilot</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            {pilots}
                        </tbody>
                    </table>
                </div>
            )
        } else {
            content = "You have no pilots on the waitlist."
        }
        return (
            <div>
                <h5 className="pb-4">Your pilots on the Waitlist</h5>
                {content}
            </div>
        )
    }
}