import React, { Component } from 'react';
import Card from './Card';

export default class WaitingPilot extends Component {

    removePilot(pilot_id) {
        $.ajax({
            type: 'delete',
            url: `${this.props.baseUri}/leave`,
            data: {
                pilot_id: pilot_id
            }
        }).done(() => {
            this.props.forceUpdate
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
                        <td><img width="32px" src={`https://image.eveonline.com/Character/${waiting.pilotId}_32.jpg`} /></td>
                        <td>{waiting.pilot.characterName}</td>
                        <td><buton className="btn btn-danger" onClick={this.removePilot.bind(this, waiting.pilotId)}>Remove <i className="fas fa-user-times"></i></buton></td>
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
                <Card heading="Your pilots on the Waitlist">
                    {content}
                </Card>
            </div>
        )
    }
}